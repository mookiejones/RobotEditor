using System;
using RobotEditor.Controls.AngleConverter.Exceptions;

namespace RobotEditor.Controls.AngleConverter.Classes
{
    [Serializable]
    public class SquareMatrix : Matrix
    {
        private const double EPSILON = 0.0001;

        protected SquareMatrix()
            : base(0, 0)
        {
        }

        public SquareMatrix(Matrix mat)
        {
            if (mat.Rows != mat.Columns)
            {
                throw new MatrixException("Matrix is not square. Cannot cast to a SquareMatrix");
            }
            Size = mat.Rows;
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    base[i, j] = mat[i, j];
                }
            }
        }

        public SquareMatrix(SquareMatrix mat)
            : base(mat)
        {
        }

        public SquareMatrix(int size)
            : base(size, size)
        {
        }

        public SquareMatrix(int size, params double[] elements)
            : base(size, size, elements)
        {
        }

        public int Size
        {
            get => Rows;
            set => SetSize(value, value);
        }

        public double Determinant()
        {
            var squareMatrix = new SquareMatrix(this);
            var num = squareMatrix.MakeRowEchelon();
            double result;
            for (var i = 0; i < Size; i++)
            {
                if (squareMatrix.IsRowZero(i))
                {
                    result = 0.0;
                    return result;
                }
            }
            result = num;
            return result;
        }

        public static SquareMatrix Identity(int size)
        {
            var squareMatrix = new SquareMatrix(size);
            for (var i = 0; i < size; i++)
            {
                squareMatrix[i, i] = 1.0;
            }
            return squareMatrix;
        }

        public SquareMatrix Inverse()
        {
            var matrix = base.Augment(Identity(Size));
            matrix.MakeRowEchelon();
            var squareMatrix = new SquareMatrix(Size);
            var squareMatrix2 = new SquareMatrix(Size);
            for (var i = 0; i < Size; i++)
            {
                squareMatrix.SetColumn(i, matrix.GetColumn(i));
                squareMatrix2.SetColumn(i, matrix.GetColumn(i + Size));
            }
            for (var j = 0; j < squareMatrix.Rows; j++)
            {
                if (squareMatrix.IsRowZero(j))
                {
                    throw new MatrixException("Matrix is singular");
                }
            }
            for (var k = Size - 1; k > 0; k--)
            {
                for (var l = k - 1; l >= 0; l--)
                {
                    var scalar = -squareMatrix[l, k];
                    squareMatrix.AddRowTimesScalar(l, k, scalar);
                    squareMatrix2.AddRowTimesScalar(l, k, scalar);
                }
            }
            return squareMatrix2;
        }

        public bool IsRotationMatrix()
        {
            bool result;
            if (!base.IsNaN())
            {
                if (Math.Abs(Determinant() - 1.0) > 0.001)
                {
                    result = false;
                    return result;
                }
                Matrix matrix = Inverse();
                Matrix matrix2 = Transpose();
                for (var i = 0; i < Size; i++)
                {
                    for (var j = 0; j < Size; j++)
                    {
                        if (Math.Abs(matrix[i, j] - matrix2[i, j]) > 0.001)
                        {
                            result = false;
                            return result;
                        }
                    }
                }
            }
            result = true;
            return result;
        }

        public void LUDecomposition(out SquareMatrix l, out SquareMatrix u)
        {
            l = new SquareMatrix(Size);
            u = new SquareMatrix(Size);
            if (Math.Abs(base[0, 0] - 0.0) < 0.0001)
            {
                throw new MatrixException("Unable to decompose matrix");
            }
            l.SetColumn(0, base.GetColumn(0));
            u.SetRow(0, base.GetRow(0));
            u.MultiplyRow(0, 1.0/base[0, 0]);
            for (var i = 1; i < Size; i++)
            {
                var array = new Vector[Size];
                var array2 = new Vector[Size];
                for (var j = 1; j < Size; j++)
                {
                    array[j] = new Vector(i);
                    array2[j] = new Vector(i);
                    var row = l.GetRow(j);
                    var column = u.GetColumn(j);
                    for (var k = 0; k < i; k++)
                    {
                        array[j][k] = row[k];
                        array2[j][k] = column[k];
                    }
                }
                for (var m = i; m < Size; m++)
                {
                    l[m, i] = base[m, i] - Vector.Dot(array[m], array2[i]);
                    if (m == i)
                    {
                        u[i, m] = 1.0;
                    }
                    else
                    {
                        if (Math.Abs(l[i, i] - 0.0) < 0.0001)
                        {
                            throw new MatrixException("Unable to decompose matrix");
                        }
                        u[i, m] = (base[i, m] - Vector.Dot(array[i], array2[m]))/l[i, i];
                    }
                }
            }
        }

        public SquareMatrix Minor(int i, int j)
        {
            var squareMatrix = new SquareMatrix(Size - 1);
            var num = 0;
            for (var k = 0; k < base.Rows; k++)
            {
                if (k != i)
                {
                    var num2 = 0;
                    for (var l = 0; l < base.Columns; l++)
                    {
                        if (l != j)
                        {
                            squareMatrix[num, num2] = base[k, l];
                            num2++;
                        }
                    }
                    num++;
                }
            }
            return squareMatrix;
        }

        public static SquareMatrix NaN(int size) => new SquareMatrix(NaN(size, size));

        public SquareMatrix Power(int power)
        {
            Matrix matrix = new SquareMatrix(this);
            for (var i = 1; i < power; i++)
            {
                matrix = this*matrix;
            }
            return new SquareMatrix(matrix);
        }

        public double Trace()
        {
            var num = 0.0;
            for (var i = 0; i < Size; i++)
            {
                num += base[i, i];
            }
            return num;
        }

        public new SquareMatrix Transpose() => new SquareMatrix(base.Transpose());
    }
}