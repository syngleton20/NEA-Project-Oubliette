using NEA_Project_Oubliette.Maps;
using System.Collections.Generic;
using System.IO;
using System;

namespace NEA_Project_Oubliette.Editing
{
    ///<summary>Provides methods and data allowing authors to easily edit maps</summary>
    internal static class MapEditor
    {
        private static bool hasSaved, hasEverSaved;
        private static string currentMapName;

        public static bool HasSaved => hasSaved;
        public static string CurrentMapName => currentMapName;

        ///<summary>Loads the template 'Untitled' map so that authors can begin editing</summary>
        public static void New()
        {
            Placement.Reset();

            currentMapName = "";
            hasSaved = hasEverSaved = false;

            Game.Current = new Game(GameType.Editor, "Untitled.map");
            Game.Current.Start();
        }

        ///<summary>Saves a map to a file, overriding it if necessary</summary>
        public static void Save()
        {
            if(hasEverSaved)
            {
                if(hasSaved) return;

                Game.Current.CurrentMap.Name = currentMapName;
                FileHandler.WriteToFile("maps/" + currentMapName + ".map", MapFormatter.Serialize(Game.Current.CurrentMap));
                hasSaved = true;

                Display.Clear();
            }
            else SaveAs();
        }

        ///<summary>Displays a menu for saving a map to a file</summary>
        public static void SaveAs()
        {
            string illegalCharacters = "/\\.,\"\'?!:;(){}[]$%&@";
            bool isValidMapName = false;

            do
            {
                Display.Clear();
                GUI.Title("Save Map As");
                Display.WriteAtCentre($"Map name must not contain any of the following: {illegalCharacters}");

                int lastY = Console.CursorTop;
                Console.SetCursorPosition(0, Console.BufferHeight - 2);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  ENTER - Complete, ESCAPE - Cancel");

                Console.SetCursorPosition(0, lastY + 1);

                currentMapName = GUI.TextField("Map Name: ", 20);
                isValidMapName = true;

                if(string.IsNullOrEmpty(currentMapName) || string.IsNullOrWhiteSpace(currentMapName)) return;
                if(currentMapName.Length >= 20) isValidMapName = false;

                for (int i = 0; i < currentMapName.Length; i++)
                {
                    if(illegalCharacters.Contains(currentMapName[i]))
                    {
                        isValidMapName = false;
                        break;
                    }
                }
            }
            while(!isValidMapName);

            Game.Current.CurrentMap.Name = currentMapName;
            FileHandler.WriteToFile("maps/" + currentMapName + ".map", MapFormatter.Serialize(Game.Current.CurrentMap));

            hasEverSaved = hasSaved = true;
            Display.Clear();
        }

        ///<summary>Displays a menu for loading custom maps</summary>
        public static void Open()
        {
            FileInfo[] files = FileHandler.GetFilesInDirectory("maps");
            List<string> choices = new List<string>();

            Display.Clear();
            GUI.Title("Open Map");

            foreach (FileInfo file in files)
                if(file.Extension == ".map")
                    choices.Add(file.Name.Split('.')[0]);

            if(choices.Count > 0)
            {
                choices.Insert(0, "Back");
                int choice = GUI.VerticalMenu(choices.ToArray());

                if(choice > 0)
                {
                    Placement.Reset();

                    currentMapName = choices[choice];
                    hasEverSaved = hasSaved = true;

                    Game.Current.Stop();
                    Game.Current = new Game(GameType.Editor, currentMapName + ".map");
                    Game.Current.Start();
                }
            }
            else
            {
                Display.WriteAtCentre("No maps available!");
                Display.WriteAtCentre("Press any key to continue...");
            }

            Display.Clear();
        }

        ///<summary>Assigns the hasSaved field to false</summary>
        public static void MakeChange() => hasSaved = false;
    }
}