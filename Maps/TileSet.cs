using System.Collections.Generic;
using System.Text;
using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Collection of tile profiles which can read from or written to files</summary>
    internal sealed class TileSet
    {
        private List<TileProfile> profiles = new List<TileProfile>();

        public TileSet(string tileSetData)
        {
            string[] lines = tileSetData.Split('\\');

            for (int i = 0; i < lines.Length; i++)
                profiles.Add(new TileProfile(lines[i]));
        }

        ///<summary>Saves a tileset to a file in bin/Debug/netcoreapp3.1/data/</summary>
        public string Save()
        {
            StringBuilder tileSetData = new StringBuilder();

            foreach (TileProfile profile in profiles)
            {
                if(tileSetData.ToString() != "") tileSetData.Append('\\');

                tileSetData.Append(profile.Ascii);
                tileSetData.Append(':');
                tileSetData.Append(profile.Unicode);
                tileSetData.Append(':');
                tileSetData.Append(profile.IsWalkable.BoolToInt());

                if(profile.BackgroundColour != ConsoleColor.Black || profile.ForegroundColour != ConsoleColor.Gray)
                {
                    tileSetData.Append(':');
                    tileSetData.Append((int)profile.BackgroundColour);
                    tileSetData.Append(':');
                    tileSetData.Append((int)profile.ForegroundColour);
                }
            }

            return tileSetData.ToString();
        }

        ///<summary>Returns a boolean depending on whether or not a tile profile was found</summary>
        public bool TryGetProfileFromAscii(char ascii, out TileProfile output)
        {
            foreach (TileProfile profile in profiles)
            {
                if(profile.Ascii == ascii)
                {
                    output = profile;
                    return true;
                }
            }

            output = null;
            return false;
        }

        ///<summary>Returns a tile profile from an ascii character</summary>
        public TileProfile GetProfileFromAscii(char ascii)
        {
            foreach (TileProfile profile in profiles)
                if(profile.Ascii == ascii) return profile;

            return null;
        }

        public static TileSet Default => new TileSet("#:  :0\\.:░░:1:8:0\\^:░░:0:0:8");
    }
}