using System;
using System.ComponentModel;
using System.Globalization;

namespace RobotEditor.Controls.AngleConverter.Classes
{
    [Localizable(false)]
    public sealed class Point2D : IFormattable
    {
        public Point2D()
        {
            Position = new Vector2D();
        }

        public Point2D(Vector2D position)
        {
            Position = position;
        }

        public Point2D(double x, double y)
        {
            Position = new Vector2D(x, y);
        }

        public Vector2D Position { get; set; }

        public double X
        {
            get => Position[0];
            set => Position[0] = value;
        }

        public double Y
        {
            get => Position[1];
            set => Position[1] = value;
        }

        public string ToString(string format, IFormatProvider formatProvider) => string.Format("{0}, {1}", X.ToString(format, CultureInfo.InvariantCulture),
                Y.ToString(format, CultureInfo.InvariantCulture));

        private bool Equals(Point2D other) => Equals(Position, other.Position);

        public override bool Equals(object obj) => obj is object &&
                   (ReferenceEquals(this, obj) || (obj.GetType() == base.GetType() && Equals((Point2D)obj)));

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(Point2D p1, Point2D p2)
        {
            return p1 == p2;
        }

        public static bool Equals(Point2D p1, Point2D p2) => p1 == p2;

        public static bool operator !=(Point2D p1, Point2D p2)
        {
            return !(p1 == p2);
        }

        public static implicit operator Vector2D(Point2D point)
        {
            return new Vector2D(point.Position);
        }

        public static Vector2D operator -(Point2D p1, Point2D p2)
        {
            return new Vector2D(p2.X - p1.X, p2.Y - p1.Y);
        }

        public static Vector2D Subtract(Point2D p1, Point2D p2) => new Vector2D(p2.X - p1.X, p2.Y - p1.Y);

        [Localizable(false)]
        public override string ToString() => string.Format("{0:F2}, {1:F2}", X, Y);
    }
}