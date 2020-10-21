using NEA_Project_Oubliette.Entities;
using NEA_Project_Oubliette.Maps;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NEA_Project_Oubliette
{
    ///<summary>Allows entities in maps to be saved and loaded</summary>
    internal static class SaveManager
    {
        ///<summary>Saves entity data from a map into a file</summary>
        public static void Save(int index)
        {
            StringBuilder saveData = new StringBuilder();

            saveData.AppendLine(Game.Current.CurrentMap.Name);

            for (int i = 0; i < Game.Current.CurrentMap.Collection.Array.Length; i++)
            {
                if(i > 0) saveData.Append('\\');
                saveData.Append(Game.Current.CurrentMap.Collection.Array[i].Save());
            }

            FileHandler.WriteToFile("saves/save_" + index.ToString() + ".sav", saveData.ToString());
        }

        ///<summary>Loads entity data into a map from a file</summary>
        public static void Load(int index)
        {
            if(!FileHandler.FileExists("saves/save_" + index.ToString() + ".sav")) return;

            List<Entity> entities = new List<Entity>();

            string saveData = FileHandler.ReadFile("saves/save_" + index.ToString() + ".sav");
            string[] lines = saveData.Split('\n');
            string[] entityStrings = lines[1].Split('\\');

            Game.Current.LoadMap(MapFormatter.Deserialize(FileHandler.ReadFile("maps/" + lines[0] + ".map")));
            Game.Current.CurrentMap.Collection.Clear();

            for (int i = 0; i < entityStrings.Length; i++)
            {
                switch (entityStrings[i].Split(' ')[0])
                {
                    case "P":
                        entities.Add(new Player(entityStrings[i]));
                        break;

                    case "E":
                        entities.Add(new Enemy(entityStrings[i]));
                        break;
                }
            }

            foreach (Entity entity in entities)
                Game.Current.CurrentMap.Collection.Add(entity);
        }

        ///<summary>Allows users to choose which save slot to save to by providing them with a vertical menu</summary>
        public static int ChooseSlotToSave()
        {
            GUI.Title("Choose Slot to Save");

            string[] choices = new string[11];
            choices[0] = "Back";

            for (int i = 0; i < 10; i++)
            {
                string fileName = "EMPTY";

                if(FileHandler.FileExists("saves/save_" + i + ".sav"))
                    fileName = FileHandler.ReadFile("saves/save_" + i + ".sav").Split('\n')[0];

                choices[i + 1] = Display.SplitStringOverBufferWidth($"Save Slot {(i + 1)}", fileName);
            }

            int choice = GUI.VerticalMenu(choices) - 1;

            if(FileHandler.FileExists("saves/save_" + choice + ".sav"))
            {
                GUI.Title("Are You Sure?");

                if(GUI.YesOrNo($"Are you sure you want\nto override slot {(choice + 1)}?")) return choice;
                else return -1;
            }

            return choice;
        }

        ///<summary>Allows users to choose which save slot to load from by providing them with a vertical menu</summary>
        public static int ChooseSlotToLoad()
        {
            FileInfo[] files = FileHandler.GetFilesInDirectory("saves");
            int numberOfSaveFiles = 0;

            for (int i = 0; i < files.Length; i++)
                if(files[i].Extension == ".sav")
                    numberOfSaveFiles++;

            if(numberOfSaveFiles <= 0) return -1;
            GUI.Title("Choose Slot to Load");

            string[] choices = new string[numberOfSaveFiles + 1];
            choices[0] = "Back";

            for (int i = 0; i < numberOfSaveFiles; i++)
            {
                string mapName = FileHandler.ReadFile("saves/save_" + i + ".sav").Split('\n')[0];
                choices[i + 1] = Display.SplitStringOverBufferWidth($"Save Slot {(i + 1)}", mapName);
            }

            return GUI.VerticalMenu(choices) - 1;
        }
    }
}