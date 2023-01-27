using System;
using System.ComponentModel;

namespace RobotEditor.Controls.AngleConverter.Classes
{
    [Localizable(false)]
    public sealed class Quaternion : IFormattable
    {
        public Quaternion()
        {
            X = 0.0;
            Y = 0.0;
            Z = 0.0;
            W = 0.0;
        }

        public Quaternion(Quaternion q)
        {
            X = q.X;
            Y = q.Y;
            Z = q.Z;
            W = q.W;
        }

        public Quaternion(Vector3D vector, double scalar)
        {
            Vector = vector;
            Scalar = scalar;
        }

        public Quaternion(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public double Scalar
        {
            get => W;
            set => W = value;
        }

        public Vector3D Vector
        {
            get => new Vector3D(X, Y, Z);
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }

        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            return ToString();
        }

        private bool Equals(Quaternion other)
        {
            return Equals(other);
        }

        public override bool Equals(object obj)
        {
            return obj is object &&
                   (ReferenceEquals(this, obj) || (obj.GetType() == GetType() && Equals((Quaternion)obj)));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public double Angle()
        {
            double num = Math.Acos(W) * 2.0;
            return num * 180.0 / 3.1415926535897931;
        }

        public Vector3D Axis()
        {
            new Quaternion(this).Normalise();
            double w = W;
            _ = Math.Acos(w);
            double num = Math.Sqrt(1.0 - (w * w));
            if (Math.Abs(num) < 0.0005)
            {
                num = 1.0;
            }
            return new Vector3D(X / num, Y / num, Z / num);
        }

        public Quaternion Conjugate()
        {
            return new Quaternion(-X, -Y, -Z, W);
        }

        public static Quaternion FromAxisAngle(Vector axis, double angle)
        {
            angle *= 0.017453292519943295;
            axis.Normalise();
            double num = Math.Sin(angle / 2.0);
            double w = Math.Cos(angle / 2.0);
            Quaternion quaternion = new Quaternion(axis[0] * num, axis[1] * num, axis[2] * num, w);
            quaternion.Normalise();
            return quaternion;
        }

        public Quaternion Inverse()
        {
            Quaternion quaternion = Conjugate();
            quaternion.Scalar = 1.0 / Scalar;
            return quaternion;
        }

        public double Magnitude()
        {
            return Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
        }

        public void Normalise()
        {
            double num = Magnitude();
            X /= num;
            Y /= num;
            Z /= num;
            W /= num;
        }

        public static Quaternion operator +(Quaternion q1, Quaternion q2)
        {
            return Add(q1, q2);
        }

        public static Quaternion Add(Quaternion q1, Quaternion q2)
        {
            return new Quaternion
            {
                Scalar = q1.Scalar + q2.Scalar,
                Vector = q1.Vector + q2.Vector
            };
        }

        public static explicit operator RotationMatrix3D(Quaternion q)
        {
            RotationMatrix3D rotationMatrix3D = new RotationMatrix3D();
            double num = q.X * q.X;
            double num2 = q.X * q.Y;
            double num3 = q.X * q.Z;
            double num4 = q.X * q.W;
            double num5 = q.Y * q.Y;
            double num6 = q.Y * q.Z;
            double num7 = q.Y * q.W;
            double num8 = q.Z * q.Z;
            double num9 = q.Z * q.W;
            rotationMatrix3D[0, 0] = 1.0 - (2.0 * (num5 + num8));
            rotationMatrix3D[1, 0] = 2.0 * (num2 + num9);
            rotationMatrix3D[2, 0] = 2.0 * (num3 - num7);
            rotationMatrix3D[0, 1] = 2.0 * (num2 - num9);
            rotationMatrix3D[1, 1] = 1.0 - (2.0 * (num + num8));
            rotationMatrix3D[2, 1] = 2.0 * (num6 + num4);
            rotationMatrix3D[0, 2] = 2.0 * (num3 + num7);
            rotationMatrix3D[1, 2] = 2.0 * (num6 - num4);
            rotationMatrix3D[2, 2] = 1.0 - (2.0 * (num + num5));
            return rotationMatrix3D;
        }

        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            Quaternion quaternion = new Quaternion();
            Vector3D vector3D = new Vector3D(q1.X, q1.Y, q1.Z);
            Vector3D vector3D2 = new Vector3D(q2.X, q2.Y, q2.Z);
            double w = q1.W;
            double w2 = q2.W;
            Vector3D vector3D3 = (Vector3D)((w * vector3D2) + (w2 * vector3D)) + Vector3D.Cross(vector3D, vector3D2);
            quaternion.X = vector3D3[0];
            quaternion.Y = vector3D3[1];
            quaternion.Z = vector3D3[2];
            quaternion.W = (w * w2) - Classes.Vector.Dot(vector3D, vector3D2);
            return quaternion;
        }

        public static Quaternion Multiply(Quaternion q1, Quaternion q2)
        {
            Quaternion quaternion = new Quaternion();
            Vector3D vector3D = new Vector3D(q1.X, q1.Y, q1.Z);
            Vector3D vector3D2 = new Vector3D(q2.X, q2.Y, q2.Z);
            double w = q1.W;
            double w2 = q2.W;
            Vector3D vector3D3 = (Vector3D)((w * vector3D2) + (w2 * vector3D)) + Vector3D.Cross(vector3D, vector3D2);
            quaternion.X = vector3D3[0];
            quaternion.Y = vector3D3[1];
            quaternion.Z = vector3D3[2];
            quaternion.W = (w * w2) - Classes.Vector.Dot(vector3D, vector3D2);
            return quaternion;
        }

        public static Quaternion operator -(Quaternion q1, Quaternion q2)
        {
            return Subtract(q1, q2);
        }

        public static Quaternion Subtract(Quaternion q1, Quaternion q2)
        {
            return new Quaternion
            {
                Scalar = q1.Scalar - q2.Scalar,
                Vector = q1.Vector - q2.Vector
            };
        }

        public static bool operator ==(Quaternion q1, Quaternion q2)
        {
            return q1 == q2;
        }

        public static bool operator !=(Quaternion q1, Quaternion q2)
        {
            return !(q1 == q2);
        }

        public static bool Equals(Quaternion q1, Quaternion q2)
        {
            return q1 == q2;
        }

        public override string ToString()
        {
            return string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F2}", new object[]
            {
                X,
                Y,
                Z,
                W
            });
        }
    }
}