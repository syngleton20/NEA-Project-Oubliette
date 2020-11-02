using NEA_Project_Oubliette.Entities;
using NEA_Project_Oubliette.Maps;
using System.Collections.Generic;
using System.Linq;
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
            if(!hasSaved && currentMapName != "")
            {
                GUI.Title("New Map");
                if(!GUI.YesOrNo("Are you sure you want to create a new map without saving?\nYour changes will not be saved."))
                {
                    Display.Clear();
                    return;
                }
            }

            Reset();

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
                choices = (from choice in choices orderby choice select choice).ToList();
                choices.Insert(0, "Back");

                int choiceIndex = GUI.VerticalMenu(choices.ToArray());

                if(choiceIndex > 0)
                {
                    if(!hasSaved && hasEverSaved)
                    {
                        GUI.Title("Exiting Without Saving");

                        if(!GUI.YesOrNo("Are you sure you want to open another map without saving?\nYour changes will not be saved."))
                            return;
                    }

                    Placement.Reset();

                    currentMapName = choices[choiceIndex];
                    hasEverSaved = hasSaved = true;

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

        ///<summary>Opens a map using a map name</summary>
        public static void Open(string mapName)
        {
            Display.Clear();

            Game.Current = new Game(GameType.Editor, mapName);
            Game.Current.Start();
        }

        ///<summary>Generates a temporary map file and starts a new game allowing the author to test their map</summary>
        public static void EnterPlayMode()
        {
            Placement.Reset();
            Display.Clear();

            FileHandler.WriteToFile("maps/.temp.map", MapFormatter.Serialize(Game.Current.CurrentMap));

            Game.Current = new Game(GameType.Test, ".temp.map");
            Game.Current.Start();
        }

        ///<summary>Stops testing the current map, attempting to load the temporary file generated by EnterPlayMode(), loads the last saved version of this map in case the temporary file cannot be found</summary>
        public static void ExitPlayMode()
        {
            Display.Clear();

            Game.Current = new Game(GameType.Editor, FileHandler.FileExists("maps/.temp.map") ? ".temp.map" : currentMapName + ".map");
            Game.Current.Start();
        }

        ///<summary>Displays a menu containing various debugging options to assist authors in the creation of maps</summary>
        public static void DebuggingMenu()
        {
            Display.Clear();
            GUI.Title("Debugging");

            switch (GUI.VerticalMenu("Back", "Show Entity List"))
            {
                case 0:
                    Display.Clear();
                    break;

                case 1:
                    Entity[] entities = Game.Current.CurrentMap.Collection.Array;
                    IEnumerable<Entity> orderedEntities = from entity in entities orderby entity.Id select entity;

                    string[] entityStrings = new string[entities.Length];
                    entities = orderedEntities.ToArray();

                    Display.Clear();
                    GUI.Title("Entity List");

                    if(entities.Length > 0)
                    {
                        for (int i = 0; i < entities.Length; i++)
                            entityStrings[i] = Display.SplitStringOverBufferWidth(EntityCollection.Names[entities[i].Save()[0]], entities[i].Save());

                        GUI.VerticalScrollView(entityStrings);
                    }
                    else Display.WriteAtCentre("No entities in map!");

                    Display.WriteAtCentre("Press any key to continue...");
                    Input.GetKeyDown();
                    Display.Clear();
                    break;
            }
        }

        ///<summary>Fills the area containing the placement point with a tile (provided via user input)</summary>
        public static void Fill()
        {
            bool isValid = true;
            char fillCharacter = '.';

            do
            {
                Display.Clear();
                GUI.Title("Fill Area");
                Display.WriteAtCentre("Enter a character to fill this area with.");

                string input = GUI.TextField("Fill Character: ", 1);
                if(string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input)) break;

                fillCharacter = input[0];
                if(!Game.Current.CurrentMap.TileSet.TryGetProfileFromAscii(fillCharacter, out TileProfile profile)) isValid = false;
            }
            while(!isValid);

            Game.Current.CurrentMap.Fill(Placement.Position.X / Map.AREA_SIZE, Placement.Position.Y / Map.AREA_SIZE, fillCharacter);
            Display.Clear();
        }

        ///<summary>Resets all necessary fields back to their default values</summary>
        public static void Reset()
        {
            Placement.Reset();

            currentMapName = "";
            hasSaved = hasEverSaved = false;
        }

        ///<summary>Assigns the hasSaved field to false</summary>
        public static void MakeChange() => hasSaved = false;
    }
}