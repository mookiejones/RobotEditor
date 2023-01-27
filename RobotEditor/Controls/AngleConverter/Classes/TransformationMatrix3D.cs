using RobotEditor.Controls.AngleConverter.Exceptions;
using System;
using System.Collections.ObjectModel;

namespace RobotEditor.Controls.AngleConverter.Classes
{
    [Serializable]
    public sealed class TransformationMatrix3D : SquareMatrix
    {
        private TransformationMatrix3D()
            : base(4)
        {
            for (int i = 0; i < 4; i++)
            {
                base[i, i] = 1.0;
            }
        }

        private TransformationMatrix3D(Matrix mat)
            : base(4)
        {
            if (mat.Rows != 4 || mat.Columns != 4)
            {
                throw new MatrixException("Matrix is not the correct size to convert to a TransformationMatrix3D");
            }
            SquareMatrix squareMatrix = new SquareMatrix(3);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    squareMatrix[i, j] = mat[i, j];
                }
            }
            if (!squareMatrix.IsRotationMatrix())
            {
                throw new MatrixException("Matrix does not contain a valid rotation");
            }
            for (int k = 0; k < 4; k++)
            {
                for (int l = 0; l < 4; l++)
                {
                    base[k, l] = mat[k, l];
                }
            }
        }

        public TransformationMatrix3D(RotationMatrix3D rot)
            : base(4)
        {
            base[3, 3] = 1.0;
            Rotation = rot;
        }

        public TransformationMatrix3D(Vector3D trans)
            : base(4)
        {
            for (int i = 0; i < 4; i++)
            {
                base[i, i] = 1.0;
            }
            Translation = trans;
        }

        public TransformationMatrix3D(Vector3D trans, RotationMatrix3D rot)
            : base(4)
        {
            for (int i = 0; i < 4; i++)
            {
                base[i, i] = 1.0;
            }
            Rotation = rot;
            Translation = trans;
        }

        public RotationMatrix3D Rotation
        {
            get
            {
                RotationMatrix3D rotationMatrix3D = new RotationMatrix3D();
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        rotationMatrix3D[i, j] = base[i, j];
                    }
                }
                return rotationMatrix3D;
            }
            set
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        base[i, j] = value[i, j];
                    }
                }
            }
        }

        public Vector3D Translation
        {
            get => new Vector3D(base[0, 3], base[1, 3], base[2, 3]);
            set
            {
                base[0, 3] = value[0];
                base[1, 3] = value[1];
                base[2, 3] = value[2];
            }
        }

        public static TransformationMatrix3D FromXYZABC(double x, double y, double z, double a, double b, double c)
        {
            return new TransformationMatrix3D(new Vector3D(x, y, z), RotationMatrix3D.FromABC(a, b, c));
        }

        public static TransformationMatrix3D FromXYZEulerZYZ(double x, double y, double z, double z1, double y1,
            double z2)
        {
            return new TransformationMatrix3D(new Vector3D(x, y, z), RotationMatrix3D.FromEulerZYZ(z1, y1, z2));
        }

        public static TransformationMatrix3D FromXYZRPY(double x, double y, double z, double r, double p, double w)
        {
            return new TransformationMatrix3D(new Vector3D(x, y, z), RotationMatrix3D.FromRPY(r, p, w));
        }

        public static TransformationMatrix3D Identity()
        {
            TransformationMatrix3D transformationMatrix3D = new TransformationMatrix3D();
            for (int i = 0; i < 4; i++)
            {
                transformationMatrix3D[i, i] = 1.0;
            }
            return transformationMatrix3D;
        }

        public new TransformationMatrix3D Inverse()
        {
            return new TransformationMatrix3D(base.Inverse());
        }

        public static TransformationMatrix3D NaN()
        {
            return new TransformationMatrix3D(NaN(4));
        }

        public static Collection<Point3D> operator *(TransformationMatrix3D transform, Collection<Point3D> points)
        {
            Collection<Point3D> collection = new Collection<Point3D>();
            foreach (Point3D current in points)
            {
                collection.Add(transform * current);
            }
            return collection;
        }

        public static Collection<Point3D> Multiply(TransformationMatrix3D transform, Collection<Point3D> points)
        {
            Collection<Point3D> collection = new Collection<Point3D>();
            foreach (Point3D current in points)
            {
                collection.Add(transform * current);
            }
            return collection;
        }

        public static Collection<Vector3D> operator *(TransformationMatrix3D transform, Collection<Vector3D> vectors)
        {
            Collection<Vector3D> collection = new Collection<Vector3D>();
            foreach (Vector3D current in vectors)
            {
                collection.Add(transform * current);
            }
            return collection;
        }

        public static Point3D operator *(TransformationMatrix3D mat, Point3D pt)
        {
            Vector3D vec = new Vector3D(pt.X, pt.Y, pt.Z);
            Vector3D vector3D = mat * vec;
            return new Point3D(vector3D.X, vector3D.Y, vector3D.Z);
        }

        public static TransformationMatrix3D operator *(TransformationMatrix3D m1, TransformationMatrix3D m2)
        {
            return new TransformationMatrix3D(m1 * m2);
        }

        public static Vector3D operator *(TransformationMatrix3D mat, Vector3D vec)
        {
            Vector vec2 = new Vector(4, new[]
            {
                vec[0],
                vec[1],
                vec[2],
                1.0
            });
            Matrix matrix = mat * vec2;
            return new Vector3D(matrix[0, 0], matrix[1, 0], matrix[2, 0]);
        }

        public string ToString(string format)
        {
            return ToString(format, null);
        }

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            string result;
            if (format.ToUpperInvariant().StartsWith("RPY"))
            {
                Vector3D translation = Translation;
                Vector3D rPY = Rotation.RPY;
                result = string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F2}, {4:F2}, {5:F2}", new object[]
                {
                    translation.X,
                    translation.Y,
                    translation.Z,
                    rPY.X,
                    rPY.Y,
                    rPY.Z
                });
            }
            else
            {
                if (format.ToUpperInvariant().StartsWith("ABC"))
                {
                    Vector3D translation2 = Translation;
                    Vector3D aBC = Rotation.ABC;
                    result = string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F2}, {4:F2}, {5:F2}", new object[]
                    {
                        translation2.X,
                        translation2.Y,
                        translation2.Z,
                        aBC.X,
                        aBC.Y,
                        aBC.Z
                    });
                }
                else
                {
                    if (format.ToUpperInvariant().StartsWith("QUATERNION"))
                    {
                        Vector3D translation3 = Translation;
                        Quaternion quaternion = (Quaternion)Rotation;
                        result = string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F3}, {4:F3}, {5:F3}, {6:F3}", new object[]
                        {
                            translation3.X,
                            translation3.Y,
                            translation3.Z,
                            quaternion.X,
                            quaternion.Y,
                            quaternion.Z,
                            quaternion.W
                        });
                    }
                    else
                    {
                        if (format.ToUpperInvariant().StartsWith("ABBQUATERNION"))
                        {
                            Vector3D translation4 = Translation;
                            Quaternion quaternion2 = (Quaternion)Rotation;
                            result = string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F3}, {4:F3}, {5:F3}, {6:F3}",
                                new object[]
                                {
                                    translation4.X,
                                    translation4.Y,
                                    translation4.Z,
                                    quaternion2.W,
                                    quaternion2.X,
                                    quaternion2.Y,
                                    quaternion2.Z
                                });
                        }
                        else
                        {
                            if (format.ToUpperInvariant().StartsWith("ABG"))
                            {
                                Vector3D translation5 = Translation;
                                Vector3D aBG = Rotation.ABG;
                                result = string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F2}, {4:F2}, {5:F2}", new object[]
                                {
                                    translation5.X,
                                    translation5.Y,
                                    translation5.Z,
                                    aBG.X,
                                    aBG.Y,
                                    aBG.Z
                                });
                            }
                            else
                            {
                                if (format.ToUpperInvariant().StartsWith("EULERZYZ"))
                                {
                                    Vector3D translation6 = Translation;
                                    Vector3D eulerZYZ = Rotation.EulerZYZ;
                                    result = string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F2}, {4:F2}, {5:F2}",
                                        new object[]
                                        {
                                            translation6.X,
                                            translation6.Y,
                                            translation6.Z,
                                            eulerZYZ.X,
                                            eulerZYZ.Y,
                                            eulerZYZ.Z
                                        });
                                }
                                else
                                {
                                    result = base.ToString(format, formatProvider);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}