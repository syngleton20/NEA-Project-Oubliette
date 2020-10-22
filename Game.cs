using NEA_Project_Oubliette.Entities;
using NEA_Project_Oubliette.Maps;
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

            while (isPlaying)
            {
                frame++;

                Console.SetCursorPosition(0, 0);
                currentMap.Draw(cameraPosition.X, cameraPosition.Y);
                currentMap.Collection.UpdateAll();

                Input.GetPlayerInput();
            }
        }

        public void Stop() => isPlaying = false;

        ///<summary>Assigns to the current map</summary>
        public void LoadMap(Map map) => currentMap = map;

        ///<summary>Displays a Game Over screen then ends this game</summary>
        public void GameOver()
        {
            isPlaying = false;
            currentMap.Collection.Clear();
            currentMap = null;

            Display.Clear();
            GUI.Title("Game Over");

            switch(GUI.VerticalMenu("Load", "Main Menu", "Quit"))
            {
                case 0:
                    SaveManager.Load(SaveManager.ChooseSlotToLoad());
                    Start();
                    break;

                case 1:
                    Debug.Test(); // this is temporary
                    break;

                case 2:
                    Environment.Exit(0); // this is also temporary
                    break;
            }
        }

        ///<summary>Moves the camera to a specified location, so only that part of the map is drawn to the screen</summary>
        public void SetCameraPosition(int cameraX, int cameraY) => cameraPosition = new Vector(cameraX, cameraY);

        public void Dispose()
        {
            currentMap.Collection.Clear();
            currentMap = null;
        }
    }
}