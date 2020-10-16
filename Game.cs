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

        public Map CurrentMap;

        public Game(GameType gameType, string mapName)
        {
            type = gameType;
            currentMap = new Map(mapName, FileHandler.ReadFile("maps/" + mapName), TileSet.Default);
        }

        ///<summary>Begins the game loop, handling drawing maps and receiving user input</summary>
        public void Start()
        {
            isPlaying = true;

            while (isPlaying)
            {
                Console.SetCursorPosition(0, 0);
                currentMap.Draw(cameraPosition.X, cameraPosition.Y);

                if(type == GameType.Game || type == GameType.Editor)
                {
                    switch (Input.GetKeyDown())
                    {
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            // Move player up
                            break;

                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            // Move player down
                            break;

                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            // Move player left
                            break;

                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                            // Move player right
                            break;
                    }
                }
            }
        }
    }
}