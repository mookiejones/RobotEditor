using System;
using RobotEditor.Controls.AngleConverter.Classes;

namespace RobotEditor.Controls.AngleConverter
{
    public sealed class RotationMatrix3D : SquareMatrix
    {
        public RotationMatrix3D()
            : base(3)
        {
            for (var i = 0; i < 3; i++)
            {
                base[i, i] = 1.0;
            }
        }

        public RotationMatrix3D(Matrix mat)
            : base(3)
        {
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    base[i, j] = mat[i, j];
                }
            }
        }

        public Vector3D ABC => new Vector3D(RPY.Z, RPY.Y, RPY.X);

        public Vector3D ABG
        {
            get
            {
                var num = Math.Asin(base[0, 2]);
                var num2 = Math.Cos(num);
                var y = num*180.0/3.1415926535897931;
                double x2;
                double z;
                if (Math.Abs(num2) > 0.005)
                {
                    var x = base[2, 2]/num2;
                    var y2 = -base[1, 2]/num2;
                    x2 = Math.Atan2(y2, x)*180.0/3.1415926535897931;
                    x = base[0, 0]/num2;
                    y2 = -base[0, 1]/num2;
                    z = Math.Atan2(y2, x)*180.0/3.1415926535897931;
                }
                else
                {
                    x2 = 0.0;
                    var x = base[1, 1];
                    var y2 = base[1, 0];
                    z = Math.Atan2(y2, x)*180.0/3.1415926535897931;
                }
                return new Vector3D(x2, y, z);
            }
        }

        public Vector3D EulerZYZ
        {
            get
            {
                var num = Math.Atan2(Math.Sqrt(Math.Pow(base[2, 0], 2.0) + Math.Pow(base[2, 1], 2.0)), base[2, 2]);
                double num2;
                double num3;
                if (Math.Abs(num) < 1E-06)
                {
                    num2 = 0.0;
                    num3 = Math.Atan2(-base[0, 1], base[0, 0]);
                }
                else
                {
                    if (Math.Abs(num - 3.1415926535897931) < 1E-06)
                    {
                        num2 = 0.0;
                        num3 = Math.Atan2(base[0, 1], -base[0, 0]);
                    }
                    else
                    {
                        num2 = Math.Atan2(base[1, 2], base[0, 2]);
                        num3 = Math.Atan2(base[2, 1], -base[2, 0]);
                    }
                }
                num2 *= 57.295779513082323;
                num *= 57.295779513082323;
                return new Vector3D(num2, num, num3*57.295779513082323);
            }
        }

        public Vector3D RPY
        {
            get
            {
                var num = Math.Atan2(base[1, 0], base[0, 0]);
                var num2 = Math.Atan2(-base[2, 0], Math.Sqrt(base[2, 1]*base[2, 1] + base[2, 2]*base[2, 2]));
                var num3 = Math.Atan2(base[2, 1], base[2, 2]);
                num *= 57.295779513082323;
                num2 *= 57.295779513082323;
                return new Vector3D(num3*57.295779513082323, num2, num);
            }
        }

        public static RotationMatrix3D FromABC(double a, double b, double c) => FromRPY(c, b, a);

        public static RotationMatrix3D FromEulerZYZ(double x, double y, double z)
        {
            var rotationMatrix3D = new RotationMatrix3D();
            var num = Math.Cos(x*3.1415926535897931/180.0);
            var num2 = Math.Cos(y*3.1415926535897931/180.0);
            var num3 = Math.Cos(z*3.1415926535897931/180.0);
            var num4 = Math.Sin(x*3.1415926535897931/180.0);
            var num5 = Math.Sin(y*3.1415926535897931/180.0);
            var num6 = Math.Sin(z*3.1415926535897931/180.0);
            rotationMatrix3D[0, 0] = num*num2*num3 - num4*num6;
            rotationMatrix3D[0, 1] = -num*num2*num6 - num4*num3;
            rotationMatrix3D[0, 2] = num*num5;
            rotationMatrix3D[1, 0] = num4*num2*num3 + num*num6;
            rotationMatrix3D[1, 1] = -num4*num2*num6 + num*num3;
            rotationMatrix3D[1, 2] = num4*num5;
            rotationMatrix3D[2, 0] = -num5*num3;
            rotationMatrix3D[2, 1] = num5*num6;
            rotationMatrix3D[2, 2] = num2;
            return rotationMatrix3D;
        }

        public static RotationMatrix3D FromRPY(double roll, double pitch, double yaw) => new RotationMatrix3D(RotateZ(yaw) * RotateY(pitch) * RotateX(roll));

        public static RotationMatrix3D Identity() => new RotationMatrix3D(Identity(3));

        public new RotationMatrix3D Inverse() => new RotationMatrix3D(base.Inverse());

        public static explicit operator Quaternion(RotationMatrix3D mat)
        {
            var quaternion = new Quaternion();
            var num = mat.Trace() + 1.0;
            if (num > 1E-05)
            {
                var num2 = Math.Sqrt(num)*2.0;
                quaternion.X = (mat[2, 1] - mat[1, 2])/num2;
                quaternion.Y = (mat[0, 2] - mat[2, 0])/num2;
                quaternion.Z = (mat[1, 0] - mat[0, 1])/num2;
                quaternion.W = 0.25*num2;
            }
            else
            {
                if (mat[0, 0] > mat[1, 1] && mat[0, 0] > mat[2, 2])
                {
                    var num3 = Math.Sqrt(1.0 + mat[0, 0] - mat[1, 1] - mat[2, 2])*2.0;
                    quaternion.X = 0.25*num3;
                    quaternion.Y = (mat[0, 1] + mat[1, 0])/num3;
                    quaternion.Z = (mat[2, 0] + mat[0, 2])/num3;
                    quaternion.W = (mat[1, 2] - mat[2, 1])/num3;
                }
                else
                {
                    if (mat[1, 1] > mat[2, 2])
                    {
                        var num4 = Math.Sqrt(1.0 + mat[1, 1] - mat[0, 0] - mat[2, 2])*2.0;
                        quaternion.X = (mat[0, 1] + mat[1, 0])/num4;
                        quaternion.Y = 0.25*num4;
                        quaternion.Z = (mat[1, 2] + mat[2, 1])/num4;
                        quaternion.W = (mat[2, 0] - mat[0, 2])/num4;
                    }
                    else
                    {
                        var num5 = Math.Sqrt(1.0 + mat[2, 2] - mat[0, 0] - mat[1, 1])*2.0;
                        quaternion.X = (mat[2, 0] + mat[0, 2])/num5;
                        quaternion.Y = (mat[1, 2] + mat[2, 1])/num5;
                        quaternion.Z = 0.25*num5;
                        quaternion.W = (mat[0, 1] - mat[1, 0])/num5;
                    }
                }
            }
            quaternion.Normalise();

            return quaternion;
        }

        public static RotationMatrix3D RotateAroundVector(Vector3D vector, double angle) => (RotationMatrix3D)Quaternion.FromAxisAngle(vector, angle);

        private static RotationMatrix3D RotateX(double angle)
        {
            angle *= 0.017453292519943295;
            var rotationMatrix3D = new RotationMatrix3D();
            rotationMatrix3D[0, 0] = 1.0;
            rotationMatrix3D[1, 1] = Math.Cos(angle);
            rotationMatrix3D[1, 2] = -Math.Sin(angle);
            rotationMatrix3D[2, 1] = Math.Sin(angle);
            rotationMatrix3D[2, 2] = Math.Cos(angle);
            return rotationMatrix3D;
        }

        private static RotationMatrix3D RotateY(double angle)
        {
            angle *= 0.017453292519943295;
            var rotationMatrix3D = new RotationMatrix3D();
            rotationMatrix3D[0, 0] = Math.Cos(angle);
            rotationMatrix3D[0, 2] = Math.Sin(angle);
            rotationMatrix3D[1, 1] = 1.0;
            rotationMatrix3D[2, 0] = -Math.Sin(angle);
            rotationMatrix3D[2, 2] = Math.Cos(angle);
            return rotationMatrix3D;
        }

        private static RotationMatrix3D RotateZ(double angle)
        {
            angle *= 0.017453292519943295;
            var rotationMatrix3D = new RotationMatrix3D();
            rotationMatrix3D[0, 0] = Math.Cos(angle);
            rotationMatrix3D[0, 1] = -Math.Sin(angle);
            rotationMatrix3D[1, 0] = Math.Sin(angle);
            rotationMatrix3D[1, 1] = Math.Cos(angle);
            rotationMatrix3D[2, 2] = 1.0;
            return rotationMatrix3D;
        }

        public double RotationAngle() => Math.Acos((base.Trace() - 1.0) / 2.0) * 180.0 / 3.1415926535897931;

        public Vector3D RotationAxis() => ((Quaternion)this).Axis();
    }
}