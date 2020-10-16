using System.Collections.Generic;
using System.Text;
using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Collection of tile profiles which can read from or written to files</summary>
    internal sealed class TileSet
    {
        private List<TileProfile> profiles = new List<TileProfile>();
        private static TileSet defaultTileSet;

        ///<summary>Saves a tileset to a file in bin/Debug/netcoreapp3.1/data/</summary>
        public void Save(string fileName)
        {
            StringBuilder profileString = new StringBuilder();

            foreach (TileProfile profile in profiles)
            {
                if(profileString.ToString() != "") profileString.Append('\n');

                profileString.Append(profile.Ascii);
                profileString.Append(',');
                profileString.Append(profile.Unicode);

                if(profile.BackgroundColour != ConsoleColor.Black || profile.ForegroundColour != ConsoleColor.Gray)
                {
                    profileString.Append(',');
                    profileString.Append((int)profile.BackgroundColour);
                    profileString.Append((int)profile.ForegroundColour);
                }
            }

            FileHandler.WriteToFile("data/" + fileName, profileString.ToString());
        }

        ///<summary>Loads a tileset from a file in bin/Debug/netcoreapp3.1/data/</summary>
        public void Load(string fileName)
        {
            string profileString = FileHandler.ReadFile("data/" + fileName);
            string[] lines = profileString.Split('\n');

            for (int i = 0; i < lines.Length; i++)
                profiles.Add(new TileProfile(lines[i]));
        }

        ///<summary>Returns a tile profile from an ascii character</summary>
        public TileProfile GetProfileFromAscii(char ascii)
        {
            foreach (TileProfile profile in profiles)
                if(profile.Ascii == ascii) return profile;

            return null;
        }

        public static TileSet Default
        {
            get
            {
                if(defaultTileSet == null)
                {
                    defaultTileSet = new TileSet();
                    defaultTileSet.Load("default.set");
                }

                return defaultTileSet;
            }
        }
    }
}