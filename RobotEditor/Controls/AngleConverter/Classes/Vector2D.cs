using RobotEditor.Controls.AngleConverter.Exceptions;

namespace RobotEditor.Controls.AngleConverter.Classes
{
    public sealed class Vector2D : Vector
    {
        public Vector2D()
            : base(2)
        {
        }

        public Vector2D(Vector vec)
            : base(2)
        {
            base[0] = vec[0];
            base[1] = vec[1];
        }

        public Vector2D(double x, double y)
            : base(2, new[]
            {
                x,
                y
            })
        {
        }

        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();

        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1 + v2);
        }

        public static Vector2D Add(Vector2D v1, Vector2D v2) => new Vector2D(v1 + v2);

        public static bool operator ==(Vector2D v1, Vector2D v2)
        {
            return v1 == v2;
        }

        public static bool Equals(Vector2D v1, Vector2D v2) => v1 == v2;

        public static bool operator !=(Vector2D v1, Vector2D v2)
        {
            return !(v1 == v2);
        }

        public static Vector2D operator /(Vector2D vec, double scalar)
        {
            return new Vector2D(vec / scalar);
        }

        public static Vector2D Divide(Vector2D vec, double scalar) => new Vector2D(vec / scalar);
    }
}