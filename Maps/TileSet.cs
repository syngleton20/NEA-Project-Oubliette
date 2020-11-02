using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Collection of tile profiles which can read from or written to files</summary>
    internal sealed class TileSet
    {
        public static readonly TileProfile[] PROFILES = new TileProfile[]
        {
            new TileProfile('#', "  ", false),
            new TileProfile('.', "░░", true, ConsoleColor.DarkGray, ConsoleColor.Black),
            new TileProfile('^', "░░", false, ConsoleColor.Black, ConsoleColor.DarkGray),
            new TileProfile('`', "░░", true, ConsoleColor.DarkGreen, ConsoleColor.Black),
            new TileProfile('%', "▒▒", false, ConsoleColor.DarkGreen, ConsoleColor.Black)
        };

        ///<summary>Returns a boolean depending on whether or not a tile profile was found</summary>
        public static bool TryGetProfileFromAscii(char ascii, out TileProfile output)
        {
            for (int i = 0; i < PROFILES.Length; i++)
            {
                if(PROFILES[i].Ascii == ascii)
                {
                    output = PROFILES[i];
                    return true;
                }
            }

            output = null;
            return false;
        }

        ///<summary>Returns a tile profile from an ascii character</summary>
        public static TileProfile GetProfileFromAscii(char ascii)
        {
            for (int i = 0; i < PROFILES.Length; i++)
                if(PROFILES[i].Ascii == ascii) return PROFILES[i];

            return null;
        }
    }
}