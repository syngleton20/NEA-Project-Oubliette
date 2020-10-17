using System.Text;

namespace NEA_Project_Oubliette.Maps
{
    ///<summary>System for map compression using run-length encoding</summary>
    internal static class MapFormatter
    {
        ///<summary>Compresses a map into a run-length encoded string</summary>
        public static string Serialize(Map map)
        {
            StringBuilder serialized = new StringBuilder();
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

            serialized.Append(charCount > 1 ? $"{charCount}{previousCharacter}" : previousCharacter.ToString());
            return serialized.ToString();
        }

        ///<summary>Decompresses a map from a run-length encoded string</summary>
        public static Map Deserialize(string data)
        {
            StringBuilder numberString = new StringBuilder();
            StringBuilder mapData = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                if(char.IsNumber(data[i])) numberString.Append(data[i]);
                else
                {
                    if(numberString.Length > 0) mapData.Append(new string(data[i], int.Parse(numberString.ToString())));
                    else mapData.Append(data[i] != '\\' ? data[i] : '\n');

                    numberString.Clear();
                }
            }

            return new Map("", mapData.ToString(), TileSet.Default);
        }
    }
}