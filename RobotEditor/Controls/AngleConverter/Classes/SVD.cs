using System;
using System.Collections.ObjectModel;
using RobotEditor.Controls.AngleConverter.Exceptions;
using RobotEditor.Controls.AngleConverter.Interfaces;

namespace RobotEditor.Controls.AngleConverter.Classes
{
    public sealed class SVD
    {
        private const double EPSILON = 0.0001;
        private readonly Matrix _u;
        private readonly SquareMatrix _v;
        private readonly Vector _w;

        public SVD(Matrix mat)
        {
            var i = 0;
            if (mat.Rows < mat.Columns)
            {
                throw new MatrixException("Matrix must have rows >= columns");
            }
            var rows = mat.Rows;
            var columns = mat.Columns;
            var vector = new Vector(columns);
            var num = 0.0;
            var num2 = 0.0;
            var num3 = 0.0;
            var num4 = 0;
            _u = new Matrix(mat);
            _v = new SquareMatrix(columns);
            _w = new Vector(columns);
            for (var j = 0; j < columns; j++)
            {
                i = j + 1;
                vector[j] = num*num2;
                var num5 = num2 = (num = 0.0);
                if (j < rows)
                {
                    for (var k = j; k < rows; k++)
                    {
                        num += Math.Abs(_u[k, j]);
                    }
                    if (Math.Abs(num - 0.0) > 0.0001)
                    {
                        for (var l = j; l < rows; l++)
                        {
                            Matrix u;
                            int row;
                            int column;
                            (u = _u)[row = l, column = j] = u[row, column]/num;
                            num5 += _u[l, j]*_u[l, j];
                        }
                        var num6 = _u[j, j];
                        num2 = ((num6 < 0.0) ? Math.Sqrt(num5) : (-Math.Sqrt(num5)));
                        var num7 = num6*num2 - num5;
                        _u[j, j] = num6 - num2;
                        if (j != columns - 1)
                        {
                            for (var m = i; m < columns; m++)
                            {
                                num5 = 0.0;
                                for (var n = j; n < rows; n++)
                                {
                                    num5 += _u[n, j]*_u[n, m];
                                }
                                num6 = num5/num7;
                                for (var num8 = j; num8 < rows; num8++)
                                {
                                    Matrix u2;
                                    int num9;
                                    int column2;
                                    (u2 = _u)[num9 = num8, column2 = m] = u2[num9, column2] + num6*_u[num8, j];
                                }
                            }
                        }
                        for (var num10 = j; num10 < rows; num10++)
                        {
                            Matrix u;
                            int row;
                            int column;
                            (u = _u)[row = num10, column = j] = u[row, column]*num;
                        }
                    }
                }
                _w[j] = num*num2;
                num5 = (num2 = (num = 0.0));
                if (j < rows && j != columns - 1)
                {
                    for (var num11 = i; num11 < columns; num11++)
                    {
                        num += Math.Abs(_u[j, num11]);
                    }
                    if (Math.Abs(num - 0.0) > 0.0001)
                    {
                        for (var num12 = i; num12 < columns; num12++)
                        {
                            Matrix u;
                            int row;
                            int column;
                            (u = _u)[row = j, column = num12] = u[row, column]/num;
                            num5 += _u[j, num12]*_u[j, num12];
                        }
                        var num6 = _u[j, i];
                        num2 = ((num6 < 0.0) ? Math.Sqrt(num5) : (-Math.Sqrt(num5)));
                        var num7 = num6*num2 - num5;
                        _u[j, i] = num6 - num2;
                        for (var num13 = i; num13 < columns; num13++)
                        {
                            vector[num13] = _u[j, num13]/num7;
                        }
                        if (j != rows - 1)
                        {
                            for (var num14 = i; num14 < rows; num14++)
                            {
                                num5 = 0.0;
                                for (var num15 = i; num15 < columns; num15++)
                                {
                                    num5 += _u[num14, num15]*_u[j, num15];
                                }
                                for (var num16 = i; num16 < columns; num16++)
                                {
                                    Matrix u3;
                                    int row2;
                                    int column3;
                                    (u3 = _u)[row2 = num14, column3 = num16] = u3[row2, column3] + num5*vector[num16];
                                }
                            }
                        }
                        for (var num17 = i; num17 < columns; num17++)
                        {
                            Matrix u;
                            int row;
                            int column;
                            (u = _u)[row = j, column = num17] = u[row, column]*num;
                        }
                    }
                }
                num3 = Math.Max(num3, Math.Abs(_w[j]) + Math.Abs(vector[j]));
            }
            for (var num18 = columns - 1; num18 >= 0; num18--)
            {
                if (num18 < columns - 1)
                {
                    if (Math.Abs(num2 - 0.0) > 0.0001)
                    {
                        for (var num19 = i; num19 < columns; num19++)
                        {
                            _v[num19, num18] = _u[num18, num19]/_u[num18, i]/num2;
                        }
                        for (var num20 = i; num20 < columns; num20++)
                        {
                            var num5 = 0.0;
                            for (var num21 = i; num21 < columns; num21++)
                            {
                                num5 += _u[num18, num21]*_v[num21, num20];
                            }
                            for (var num22 = i; num22 < columns; num22++)
                            {
                                int num9;
                                Matrix v;
                                int row3;
                                (v = _v)[row3 = num22, num9 = num20] = v[row3, num9] + num5*_v[num22, num18];
                            }
                        }
                    }
                    for (var num23 = i; num23 < columns; num23++)
                    {
                        _v[num18, num23] = (_v[num23, num18] = 0.0);
                    }
                }
                _v[num18, num18] = 1.0;
                num2 = vector[num18];
                i = num18;
            }
            for (var num24 = columns - 1; num24 >= 0; num24--)
            {
                i = num24 + 1;
                num2 = _w[num24];
                if (num24 < columns - 1)
                {
                    for (var num25 = i; num25 < columns; num25++)
                    {
                        _u[num24, num25] = 0.0;
                    }
                }
                Matrix u2;
                int num9;
                int column2;
                if (Math.Abs(num2 - 0.0) > 0.0001)
                {
                    num2 = 1.0/num2;
                    if (num24 != columns - 1)
                    {
                        for (var num26 = i; num26 < columns; num26++)
                        {
                            var num5 = 0.0;
                            for (var num27 = i; num27 < rows; num27++)
                            {
                                num5 += _u[num27, num24]*_u[num27, num26];
                            }
                            var num6 = num5/_u[num24, num24]*num2;
                            for (var num28 = num24; num28 < rows; num28++)
                            {
                                (u2 = _u)[num9 = num28, column2 = num26] = u2[num9, column2] + num6*_u[num28, num24];
                            }
                        }
                    }
                    for (var num29 = num24; num29 < rows; num29++)
                    {
                        Matrix u;
                        int row;
                        int column;
                        (u = _u)[row = num29, column = num24] = u[row, column]*num2;
                    }
                }
                else
                {
                    for (var num30 = num24; num30 < rows; num30++)
                    {
                        _u[num30, num24] = 0.0;
                    }
                }
                (u2 = _u)[num9 = num24, column2 = num24] = u2[num9, column2] + 1.0;
            }
            for (var num31 = columns - 1; num31 >= 0; num31--)
            {
                for (var num32 = 0; num32 < 30; num32++)
                {
                    var num33 = 1;
                    for (i = num31; i >= 0; i--)
                    {
                        num4 = i - 1;
                        if (Math.Abs(Math.Abs(vector[i]) + num3 - num3) < 0.0001)
                        {
                            num33 = 0;
                            break;
                        }
                        if (Math.Abs(Math.Abs(_w[num4]) + num3 - num3) < 0.0001)
                        {
                            break;
                        }
                    }
                    double num5;
                    double num6;
                    double num7;
                    double num35;
                    double num37;
                    double num38;
                    if (num33 != 0)
                    {
                        num5 = 1.0;
                        for (var num34 = i; num34 <= num31; num34++)
                        {
                            num6 = num5*vector[num34];
                            if (Math.Abs(Math.Abs(num6) + num3 - num3) > 0.0001)
                            {
                                num2 = _w[num34];
                                num7 = Pythag(num6, num2);
                                _w[num34] = (float) num7;
                                num7 = 1.0/num7;
                                num35 = num2*num7;
                                num5 = -num6*num7;
                                for (var num36 = 0; num36 < rows; num36++)
                                {
                                    num37 = _u[num36, num4];
                                    num38 = _u[num36, num34];
                                    _u[num36, num4] = num37*num35 + num38*num5;
                                    _u[num36, num34] = num38*num35 - num37*num5;
                                }
                            }
                        }
                    }
                    num38 = _w[num31];
                    if (i == num31)
                    {
                        if (num38 < 0.0)
                        {
                            _w[num31] = -num38;
                            for (var num39 = 0; num39 < columns; num39++)
                            {
                                _v[num39, num31] = -_v[num39, num31];
                            }
                        }
                        break;
                    }
                    if (num32 >= 30)
                    {
                        throw new MatrixException("No convergence after 30 iterations");
                    }
                    var num40 = _w[i];
                    num4 = num31 - 1;
                    num37 = _w[num4];
                    num2 = vector[num4];
                    num7 = vector[num31];
                    num6 = ((num37 - num38)*(num37 + num38) + (num2 - num7)*(num2 + num7))/(2.0*num7*num37);
                    num2 = Pythag(num6, 1.0);
                    var num41 = (num6 >= 0.0) ? num2 : (-num2);
                    num6 = ((num40 - num38)*(num40 + num38) + num7*(num37/(num6 + num41) - num7))/num40;
                    num5 = (num35 = 1.0);
                    for (var num42 = i; num42 <= num4; num42++)
                    {
                        var num43 = num42 + 1;
                        num2 = vector[num43];
                        num37 = _w[num43];
                        num7 = num5*num2;
                        num2 = num35*num2;
                        num38 = Pythag(num6, num7);
                        vector[num42] = num38;
                        num35 = num6/num38;
                        num5 = num7/num38;
                        num6 = num40*num35 + num2*num5;
                        num2 = num2*num35 - num40*num5;
                        num7 = num37*num5;
                        num37 *= num35;
                        for (var num44 = 0; num44 < columns; num44++)
                        {
                            num40 = _v[num44, num42];
                            num38 = _v[num44, num43];
                            _v[num44, num42] = num40*num35 + num38*num5;
                            _v[num44, num43] = num38*num35 - num40*num5;
                        }
                        num38 = Pythag(num6, num7);
                        _w[num42] = num38;
                        if (Math.Abs(num38 - 0.0) > 0.0001)
                        {
                            num38 = 1.0/num38;
                            num35 = num6*num38;
                            num5 = num7*num38;
                        }
                        num6 = num35*num2 + num5*num37;
                        num40 = num35*num37 - num5*num2;
                        for (var num45 = 0; num45 < rows; num45++)
                        {
                            num37 = _u[num45, num42];
                            num38 = _u[num45, num43];
                            _u[num45, num42] = num37*num35 + num38*num5;
                            _u[num45, num43] = num38*num35 - num37*num5;
                        }
                    }
                    vector[i] = 0.0;
                    vector[num31] = num6;
                    _w[num31] = num40;
                }
            }
        }

        public double ConditionNumber
        {
            get
            {
                var num = double.PositiveInfinity;
                var num2 = 0.0;
                for (var i = 0; i < W.Rows; i++)
                {
                    num = Math.Min(num, _w[i]);
                    num2 = Math.Max(num2, _w[i]);
                }
                return num2/num;
            }
        }

        public int SmallestSingularIndex
        {
            get
            {
                var num = W[0];
                var result = 0;
                for (var i = 1; i < W.Size; i++)
                {
                    if (W[i] < num)
                    {
                        num = W[i];
                        result = i;
                    }
                }
                return result;
            }
        }

        public Matrix U => _u;

        public SquareMatrix V => _v;

        public Vector W => _w;

        private static double Pythag(double a, double b)
        {
            var num = Math.Abs(a);
            var num2 = Math.Abs(b);
            double result;
            if (num > num2)
            {
                var num3 = num2/num;
                result = num*Math.Sqrt(1.0 + num3*num3);
            }
            else
            {
                if (num2 > 0.0)
                {
                    var num3 = num/num2;
                    result = num2*Math.Sqrt(1.0 + num3*num3);
                }
                else
                {
                    result = 0.0;
                }
            }
            return result;
        }
    }

    public sealed class Vector3D : Vector, IGeometricElement3D, IFormattable
    {
        public Vector3D()
            : base(3)
        {
        }

        public Vector3D(Matrix mat)
            : base(3)
        {
            if (mat.Rows != 1 && mat.Columns != 1)
            {
                throw new MatrixException("Cannot convert Matrix to Vector3D. It has more than one row or column");
            }
            if (mat.Columns == 3 && mat.Rows == 1)
            {
                mat = mat.Transpose();
            }
            for (var i = 0; i < 3; i++)
            {
                base[i] = mat[i, 0];
            }
        }

        public Vector3D(Vector vec)
            : base(3)
        {
            for (var i = 0; i < 3; i++)
            {
                base[i] = vec[i];
            }
        }

        public Vector3D(double x, double y, double z)
            : base(3, new[]
            {
                x,
                y,
                z
            })
        {
        }

        public double X
        {
            get => base[0];
            set => base[0] = value;
        }

        public double Y
        {
            get => base[1];
            set => base[1] = value;
        }

        public double Z
        {
            get => base[2];
            set => base[2] = value;
        }

        TransformationMatrix3D IGeometricElement3D.Position
        {
            get { throw new NotImplementedException(); }
        }

        public override int GetHashCode() => base.GetHashCode();

        public override bool Equals(object obj) => base.Equals(obj);

        public static double Angle(Vector vec1, Vector vec2)
        {
            var num = Dot(vec1, vec2);
            var num2 = vec1.Length()*vec2.Length();
            return Math.Acos(num/num2)*180.0/3.1415926535897931;
        }

        public static Vector3D Cross(Vector vec1, Vector vec2)
        {
            var vector3D = new Vector3D();
            vector3D[0] = vec1[1]*vec2[2] - vec1[2]*vec2[1];
            vector3D[1] = vec1[2]*vec2[0] - vec1[0]*vec2[2];
            vector3D[2] = vec1[0]*vec2[1] - vec1[1]*vec2[0];
            return vector3D;
        }

        public static Vector3D NaN() => new Vector3D(NaN(3, 1));

        public new Vector3D Normalised() => new Vector3D(this / base.Length());

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1 + v2);
        }

        public static Vector3D Add(Vector3D v1, Vector3D v2) => new Vector3D(v1 + v2);

        public static Vector3D operator /(Vector3D vec, double scalar)
        {
            return new Vector3D(vec/scalar);
        }

        public static Vector3D Divide(Vector3D vec, double scalar) => new Vector3D(vec / scalar);

        public static explicit operator Point3D(Vector3D vec)
        {
            return new Point3D(vec);
        }

        public static Point3D ToPoint3D(Vector3D vec) => new Point3D(vec);

        public static Vector3D operator *(Vector3D vec, double scalar)
        {
            return new Vector3D(vec*scalar);
        }

        public static Vector3D Multiply(Vector3D vec, double scalar) => new Vector3D(vec * scalar);

        public static Collection<Vector3D> operator *(Collection<TransformationMatrix3D> transforms, Vector3D vector)
        {
            var collection = new Collection<Vector3D>();
            foreach (var current in transforms)
            {
                collection.Add(current*vector);
            }
            return collection;
        }

        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1 - v2);
        }

        public static bool operator ==(Vector3D v1, Vector3D v2)
        {
            return v1 == v2;
        }

        public static bool operator !=(Vector3D v1, Vector3D v2)
        {
            return !(v1 == v2);
        }

        public static Vector3D Subtract(Vector3D v1, Vector3D v2) => new Vector3D(v1 - v2);

        public static Vector3D operator -(Vector3D vec)
        {
            return Negate(vec);
        }

        public static Vector3D Negate(Vector3D vec) => new Vector3D(-vec.X, -vec.Y, -vec.Z);
    }
}