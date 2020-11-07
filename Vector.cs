using System;

namespace NEA_Project_Oubliette
{
    ///<summary>Structure for storing X and Y coordinates in 2D space</summary>
    internal struct Vector
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int Magnitude => (int)Math.Sqrt((X * X) + (Y * Y));

        public override int GetHashCode() => base.GetHashCode();
        public override bool Equals(object obj) => ((Vector)obj).X == X && ((Vector)obj).Y == Y;
        public override string ToString() => X.ToString() + ',' + Y.ToString();

        public static bool operator ==(Vector a, Vector b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Vector a, Vector b) => a.X != b.X || a.Y != b.Y;

        public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);
        public static Vector operator -(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y);
        public static Vector operator -(Vector v) => new Vector(-v.X, -v.Y);
        public static Vector operator *(Vector v, int m) => new Vector(v.X * m, v.Y * m);
        public static Vector operator /(Vector v, int d) => new Vector(v.X / d, v.Y / d);

        ///<summary>Clamps the X and Y axes between -1 and 1</summary>
        public Vector Normalise() => this / Magnitude;

        ///<summary>Shorthand for writing new Vector(0, 0)</summary>
        public static Vector Zero => new Vector(0, 0);

        ///<summary>Shorthand for writing new Vector(1, 1)</summary>
        public static Vector One => new Vector(1, 1);

        ///<summary>Shorthand for writing new Vector(0, 1)</summary>
        public static Vector Up => new Vector(0, 1);

        ///<summary>Shorthand for writing new Vector(0, -1)</summary>
        public static Vector Down => new Vector(0, -1);

        ///<summary>Shorthand for writing new Vector(-1, 0)</summary>
        public static Vector Left => new Vector(-1, 0);

        ///<summary>Shorthand for writing new Vector(1, 0)</summary>
        public static Vector Right => new Vector(1, 0);
    }
}