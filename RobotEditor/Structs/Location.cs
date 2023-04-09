using System;
using System.ComponentModel;

namespace RobotEditor.Structs
{
    public struct Location : IComparable<Location>, IEquatable<Location>
    {
        public static readonly Location Empty = new(-1, -1);

        public Location(int column, int line)
        {
            X = column;
            Y = line;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public int Line
        {
            get => Y;
            set => Y = value;
        }

        public int Column
        {
            get => X;
            set => X = value;
        }

        public bool IsEmpty => X <= 0 && Y <= 0;

        public int CompareTo(Location other)
        {
            int result;
            if (this == other)
            {
                result = 0;
            }
            else
            {
                result = this < other ? -1 : 1;
            }
            return result;
        }

        public bool Equals(Location other) => this == other;

        [Localizable(false)]
        public override string ToString() => string.Format("(Line {1}, Col {0})", X, Y);

        public override int GetHashCode() => (87 * X.GetHashCode()) ^ Y.GetHashCode();

        public override bool Equals(object obj) => obj is Location && (Location)obj == this;

        public static bool operator ==(Location a, Location b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Location a, Location b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static bool operator <(Location a, Location b)
        {
            return a.Y < b.Y || (a.Y == b.Y && a.X < b.X);
        }

        public static bool operator >(Location a, Location b)
        {
            return a.Y > b.Y || (a.Y == b.Y && a.X > b.X);
        }

        public static bool operator <=(Location a, Location b)
        {
            return !(a > b);
        }

        public static bool operator >=(Location a, Location b)
        {
            return !(a < b);
        }
    }
}