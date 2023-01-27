using System;
using System.ComponentModel;

namespace RobotEditor.Structs
{
    public struct Location : IComparable<Location>, IEquatable<Location>
    {
        public static readonly Location Empty = new Location(-1, -1);
        private int _x;
        private int _y;

        
        public Location(int column, int line)
        {
            _x = column;
            _y = line;
        }

        public int X
        {
            get => _x;
            set => _x = value;
        }

        public int Y
        {
            get => _y;
            set => _y = value;
        }

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
                if (this < other)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
            }
            return result;
        }

        public bool Equals(Location other) => this == other;

        [Localizable(false)]
        public override string ToString() => string.Format("(Line {1}, Col {0})", X, Y);

        public override int GetHashCode() => 87 * X.GetHashCode() ^ Y.GetHashCode();

        public override bool Equals(object obj) => obj is Location && (Location)obj == this;

        public static bool operator ==(Location a, Location b)
        {
            return a.X == b._x && a.Y == b._y;
        }

        public static bool operator !=(Location a, Location b)
        {
            return a.X != b._x || a.Y != b._y;
        }

        public static bool operator <(Location a, Location b)
        {
            return a.Y < b._y || (a.Y == b._y && a.X < b._x);
        }

        public static bool operator >(Location a, Location b)
        {
            return a.Y > b._y || (a.Y == b._y && a.X > b._x);
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