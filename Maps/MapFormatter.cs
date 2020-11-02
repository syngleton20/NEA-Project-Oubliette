using NEA_Project_Oubliette.Entities;
using System.Collections.Generic;
using System.Text;

namespace NEA_Project_Oubliette.Maps
{
    ///<summary>System for map compression using run-length encoding</summary>
    internal static class MapFormatter
    {
        ///<summary>Compresses a map into a run-length encoded string</summary>
        public static string Serialize(Map map)
        {
            StringBuilder serialized = new StringBuilder(), entities = new StringBuilder();
            string data = map.GetRawData();

            char previousCharacter = data[0];
            int charCount = 1;

            for (int i = 1; i < data.Length; i++)
            {
                if(previousCharacter != data[i])
                {
                    serialized.Append(charCount > 1 ? $"{charCount}{previousCharacter}" : previousCharacter.ToString());
                    charCount = 0;
                }

                previousCharacter = data[i];
                charCount++;
            }

            if(map.Collection.Array.Length > 0)
            {
                for (int i = 0; i < map.Collection.Array.Length; i++)
                {
                    if(i > 0) entities.Append('\\');
                    entities.Append(map.Collection.Array[i].Save());
                }
            }

            serialized.Append(charCount > 1 ? $"{charCount}{previousCharacter}" : previousCharacter.ToString());

            string finalData = map.Name + '\n' + serialized.ToString();
            if(entities.Length > 0) finalData += '\n' + entities.ToString();

            return finalData;
        }

        ///<summary>Decompresses a map from a run-length encoded string</summary>
        public static Map Deserialize(string data)
        {
            List<Entity> entities = new List<Entity>();

            StringBuilder numberString = new StringBuilder();
            StringBuilder mapData = new StringBuilder();

            string[] lines = data.Split('\n');

            for (int i = 0; i < lines[1].Length; i++)
            {
                if(char.IsNumber(lines[1][i])) numberString.Append(lines[1][i]);
                else
                {
                    if(numberString.Length > 0) mapData.Append(new string(lines[1][i], int.Parse(numberString.ToString())));
                    else mapData.Append(lines[1][i] != '\\' ? lines[1][i] : '\n');

                    numberString.Clear();
                }
            }

            if(lines.Length > 2)
            {
                string[] entityStrings = lines[2].Split('\\');

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
            }

            return new Map(lines[0], mapData.ToString(), entities.ToArray());
        }
    }
}