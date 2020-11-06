using NEA_Project_Oubliette.Editing;
using NEA_Project_Oubliette.Entities;
using NEA_Project_Oubliette.Maps;
using System.Threading;
using System.Text;
using System;

namespace NEA_Project_Oubliette
{
    public enum GameType { Game, Editor, Test }

    ///<summary>Contains information on the game in progress</summary>
    internal sealed class Game : IDisposable
    {
        private int frame;
        private bool isPlaying;

        private GameType type;
        private Map currentMap;

        private Vector cameraPosition;

        public int Frame => frame;
        public Map CurrentMap => currentMap;

        private static Game current;

        public static Game Current
        {
            get => current;
            set
            {
                if(current != null) current.Stop();
                current = value;
            }
        }

        public Game(GameType gameType, string mapName)
        {
            Current = this; // Assigns to the singleton pattern, allowing access to this particular instance

            type = gameType;
            currentMap = MapFormatter.Deserialize(FileHandler.ReadFile("maps/" + mapName));
        }

        ///<summary>Begins the game loop, handling drawing maps and receiving user input</summary>
        public void Start()
        {
            isPlaying = true;

            if(type == GameType.Editor) Placement.Reset();

            while (isPlaying)
            {
                frame++;

                if(type == GameType.Game)
                {
                    Console.SetCursorPosition(0, 1);
                    Display.WriteAtCentre(Game.Current.CurrentMap.Name);
                    GUI.HorizontalSeparator();

                    Display.DrawMap(currentMap, cameraPosition.X, cameraPosition.Y);
                    Console.ResetColor();

                    Console.WriteLine();
                    Console.WriteLine();

                    if(Player.Instance != null) GUI.HorizontalBar(Player.Instance.Health * 2, Player.Instance.MaxHealth * 2, "Health");

                    Input.GetPlayerInput();
                    currentMap?.Collection.UpdateAll();
                }
                else if(type == GameType.Editor)
                {
                    Console.SetCursorPosition(0, 1);
                    Display.WriteAtCentre(Game.Current.CurrentMap.Name + (MapEditor.HasSaved ? '\0' : '*'));
                    GUI.HorizontalSeparator();

                    Display.DrawMap(currentMap, cameraPosition.X, cameraPosition.Y);
                    Console.ResetColor();

                    Placement.Draw();

                    Console.SetCursorPosition(0, Map.AREA_SIZE + 5);
                    GUI.HorizontalSeparator();

                    Display.ClearLine();
                    Console.WriteLine();
                    Display.ClearLine();

                    Console.CursorTop--;
                    StringBuilder editorInformation = new StringBuilder();

                    editorInformation.Append($"{(Placement.Type == PlacementType.Entity ? $"Entity: {Placement.Entity.ToString()}" : $"Tile: {Placement.Tile.ToString()}")}");
                    editorInformation.Append($"  Pos: {Placement.Position.ToString()}  Stamp: {(Placement.UseStamp ? "On " : "Off")}  Size: {Game.Current.CurrentMap.Size.ToString()}");
                    editorInformation.Append($"  Area: {(Placement.Position / Map.AREA_SIZE).ToString()}");

                    if(currentMap.Collection.TryGetEntity(Placement.Position, out Entity entity))
                        editorInformation.Append("  Id: " + entity.Id);

                    Display.WriteAtCentre(editorInformation.ToString());
                    Display.WriteAtCentreBottom("WASD/ARROW KEYS - Move, SPACE - Place, Q - Stamp, ^F - Fill Area");

                    Input.GetEditorInput();
                }
                else
                {
                    Console.SetCursorPosition(0, 1);
                    Display.WriteAtCentre("▶ " + Game.Current.CurrentMap.Name);
                    GUI.HorizontalSeparator();

                    Display.DrawMap(currentMap, cameraPosition.X, cameraPosition.Y);
                    Console.ResetColor();

                    Console.WriteLine();
                    Console.WriteLine();

                    if(Player.Instance != null) GUI.HorizontalBar(Player.Instance.Health * 2, Player.Instance.MaxHealth * 2, "Health");

                    Input.GetTestInput();
                    currentMap.Collection.UpdateAll();
                }
            }
        }

        ///<summary>Assigns the isPlaying field to false</summary>
        public void Stop() => isPlaying = false;

        ///<summary>Assigns to the current map</summary>
        public void LoadMap(Map map) => currentMap = map;

        ///<summary>Displays a Game Over screen then ends this game</summary>
        public void GameOver()
        {
            isPlaying = false;

            Display.ShowRibbonMessage("G A M E    O V E R");
            Thread.Sleep(2000);

            currentMap.Collection.Clear();
            currentMap = null;

            Display.Clear();

            if(type == GameType.Test)
            {
                MapEditor.ExitPlayMode();
                return;
            }

            GUI.Title("Game Over");

            switch(GUI.VerticalMenu("Load from Save Slot", "Main Menu", "Quit"))
            {
                case 0:
                    int slotIndex = SaveManager.ChooseSlotToLoad();
                    if(slotIndex < 0) break;

                    Current = SaveManager.Load(slotIndex);
                    Current.Start();
                    break;

                case 1:
                    break;

                case 2:
                    Environment.Exit(0); // this is temporary
                    break;
            }
        }

        ///<summary>Moves the camera to a specified location, so only that part of the map is drawn to the screen</summary>
        public void SetCameraPosition(int cameraX, int cameraY) => cameraPosition = new Vector(cameraX, cameraY);

        ///<summary>Safely disposes of this game instance, disposing of its map and all its entities</summary>
        public void Dispose()
        {
            currentMap.Collection.Clear();
            currentMap = null;
        }
    }
}