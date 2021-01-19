using NEA_Project_Oubliette.Database;
using NEA_Project_Oubliette.Entities;
using NEA_Project_Oubliette.Editing;
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
        public string MapName { get; private set; }

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

            MapName = mapName;
        }

        ///<summary>Begins the game loop, handling the drawing of maps and receiving user input</summary>
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

                    Console.WriteLine();
                    Console.WriteLine();

                    Display.WriteAtCentre($"Score: {Player.Instance.Score} Equipped Item: {(Player.Instance.EquippedItem != null ? Player.Instance.EquippedItem.Name : "Nothing")}  Strength: {Player.Instance.Strength}");
                    Display.WriteAtCentreBottom("Press P for help");

                    if(Player.Instance.IsDead) GameOver();
                    else
                    {
                        Input.GetPlayerInput();
                        currentMap?.Collection.UpdateAll();
                    }
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
                    Display.WriteAtCentreBottom("Press P for help");

                    Input.GetEditorInput();
                }
                else
                {
                    Console.SetCursorPosition(0, 1);
                    Display.WriteAtCentre(Game.Current.CurrentMap.Name + " (Test Mode)");
                    GUI.HorizontalSeparator();

                    Display.DrawMap(currentMap, cameraPosition.X, cameraPosition.Y);
                    Console.ResetColor();

                    Console.WriteLine();
                    Console.WriteLine();

                    if(Player.Instance != null) GUI.HorizontalBar(Player.Instance.Health * 2, Player.Instance.MaxHealth * 2, "Health");

                    Console.WriteLine();
                    Console.WriteLine();

                    Display.WriteAtCentre($"Score: {Player.Instance.Score} Equipped Item: {(Player.Instance.EquippedItem != null ? Player.Instance.EquippedItem.Name : "Nothing")}  Strength: {Player.Instance.Strength}");
                    Display.WriteAtCentreBottom("Press P for help");

                    if(Player.Instance != null && Player.Instance.IsDead) GameOver();
                    else
                    {
                        Input.GetTestInput();
                        currentMap?.Collection.UpdateAll();
                    }
                }
            }
        }

        ///<summary>Assigns the isPlaying field to false</summary>
        public void Stop() => isPlaying = false;

        ///<summary>Assigns to the current map</summary>
        public void LoadMap(Map map) => currentMap = map;

        public void Finish()
        {
            isPlaying = false;
            Display.Clear();

            if(type == GameType.Test)
            {
                MapEditor.ExitPlayMode();
                return;
            }

            GUI.Title("Congratulations!");

            Display.WriteAtCentre("You managed to escape!");
            Display.WriteAtCentre($"Your score is {Player.Instance.Score}.");

            if(type == GameType.Game && AccountManager.IsLoggedIn && Scoreboard.IsHighScore(Player.Instance.Score, AccountManager.Account.UserID))
                if(GUI.YesOrNo($"Your score is {Player.Instance.Score}. Do you want to submit it?"))
                    Scoreboard.SubmitScore(Player.Instance.Score, AccountManager.Account.UserID);

            Display.Clear();
            GUI.Title("Congratulations!");

            Dispose();

            switch (GUI.VerticalMenu("Main Menu", "Quit"))
            {
                case 0:
                    break;

                case 1:
                    Program.QuitGame();
                    break;
            }
        }

        ///<summary>Displays a Game Over screen then ends this game</summary>
        public void GameOver()
        {
            isPlaying = false;

            Display.ShowRibbonMessage("G A M E    O V E R");
            Thread.Sleep(2000);

            Display.Clear();

            if(type == GameType.Test)
            {
                MapEditor.ExitPlayMode();
                Dispose();
                return;
            }

            GUI.Title("Game Over");

            if(type == GameType.Game && AccountManager.IsLoggedIn && Scoreboard.IsHighScore(Player.Instance.Score, AccountManager.Account.UserID))
                if(GUI.YesOrNo($"Your score is {Player.Instance.Score}. Do you want to submit it?"))
                    Scoreboard.SubmitScore(Player.Instance.Score, AccountManager.Account.UserID);

            Dispose();

            Display.Clear();
            GUI.Title("Game Over");

            switch(GUI.VerticalMenu("Load Game", "Main Menu", "Quit"))
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
                    Program.QuitGame();
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