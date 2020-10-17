using NEA_Project_Oubliette.Entities;
using NEA_Project_Oubliette.Maps;
using System;

namespace NEA_Project_Oubliette
{
    public enum GameType { Game, Editor, Test }

    ///<summary>Contains information on the game in progress</summary>
    internal sealed class Game
    {
        private bool isPlaying;

        private GameType type;
        private Map currentMap;

        private Vector cameraPosition;

        public Map CurrentMap => currentMap;
        public static Game Current { get; private set; } // This property is used for access within a singleton pattern

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
                Console.SetCursorPosition(0, 0);
                currentMap.Draw(cameraPosition.X, cameraPosition.Y);
                currentMap.Collection.UpdateAll();

                if(type == GameType.Game || type == GameType.Editor)
                {
                    switch (Input.GetKeyDown())
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            Player.Instance.Move(0, -1);
                            break;

                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            Player.Instance.Move(0, 1);
                            break;

                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            Player.Instance.Move(-1, 0);
                            break;

                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                            Player.Instance.Move(1, 0);
                            break;
                    }
                }
            }
        }

        public void GameOver()
        {
            isPlaying = false;
            currentMap.Collection.Clear();
            currentMap = null;

            Console.Clear();
            Console.WriteLine("  Game Over!");
        }

        ///<summary>Moves the camera to a specified location, so only that part of the map is drawn to the screen</summary>
        public void SetCameraPosition(int cameraX, int cameraY) => cameraPosition = new Vector(cameraX, cameraY);
    }
}