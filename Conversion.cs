using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Provides extensions methods to easily convert between string format and various data types</summary>
    internal static class Conversion
    {
        ///<summary>Converts a string to an integer</summary>
        public static int ToInt(this string source, int defaultValue = 0)
        {
            if(int.TryParse(source, out int value)) return value;
            return defaultValue;
        }

        ///<summary>Converts a string (either '0' or '1') to a boolean</summary>
        public static bool ToBool(this string source, bool defaultValue = false)
        {
            if(int.TryParse(source, out int value)) return value > 0;
            return defaultValue;
        }

        ///<summary>Converts a string to a vector (with comma delimiter)</summary>
        public static Vector ToVector(this string source)
        {
            if(source.Contains(','))
            {
                string[] axes = source.Split(',');
                return new Vector(axes[0].ToInt(), axes[1].ToInt());
            }

            return Vector.Zero;
        }
    }
}