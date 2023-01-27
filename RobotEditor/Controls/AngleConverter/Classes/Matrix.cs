using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using RobotEditor.Controls.AngleConverter.Exceptions;

namespace RobotEditor.Controls.AngleConverter.Classes
{
    [Localizable(false)]
    [Serializable]
    public class Matrix : IFormattable
    {
        private const double EPSILON = 0.0001;
        private int _columns;
        private double[,] _elements;
        private int _rows;

        protected Matrix()
        {
            SetSize(0, 0);
        }

        public Matrix(Matrix mat)
        {
            SetSize(mat.Rows, mat.Columns);
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    _elements[i, j] = mat[i, j];
                }
            }
        }

        public Matrix(int rows, int columns)
        {
            SetSize(rows, columns);
        }

        protected Matrix(int rows, int columns, params double[] elements)
        {
            if (elements.Length != rows*columns)
            {
                throw new MatrixException("Number of elements does not match matrix dimension");
            }
            SetSize(rows, columns);
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    _elements[i, j] = elements[j + i*columns];
                }
            }
        }

        public int Columns => _columns;

        public double this[int row, int column]
        {
            get => _elements[row, column];
            set => _elements[row, column] = value;
        }

        public int Rows => _rows;

        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            var stringBuilder = new StringBuilder();
            if (format == null)
            {
                format = "F2";
            }
            string result;
            if (format.ToUpper().StartsWith("F"))
            {
                for (var i = 0; i < Rows; i++)
                {
                    stringBuilder.Append("[");
                    for (var j = 0; j < Columns; j++)
                    {
                        stringBuilder.Append(this[i, j].ToString(format));
                        if (j + 1 < Columns)
                        {
                            stringBuilder.Append(", ");
                        }
                    }
                    stringBuilder.Append("]");
                    if (i + 1 < Rows)
                    {
                        stringBuilder.Append(Environment.NewLine);
                    }
                }
                result = stringBuilder.ToString();
            }
            else
            {
                if (!format.ToUpper(CultureInfo.InvariantCulture).StartsWith("MATLAB"))
                {
                    throw new FormatException("Invalid Format Specifier");
                }
                stringBuilder = new StringBuilder();
                stringBuilder.Append("[");
                for (var k = 0; k < Rows; k++)
                {
                    for (var l = 0; l < Columns; l++)
                    {
                        stringBuilder.Append(this[k, l].ToString("F5"));
                        if (l + 1 < Columns)
                        {
                            stringBuilder.Append(", ");
                        }
                    }
                    if (k + 1 < Rows)
                    {
                        stringBuilder.Append(";" + Environment.NewLine);
                    }
                }
                result = stringBuilder + "]";
            }
            return result;
        }

        private bool Equals(Matrix other) => _columns == other._columns && Equals(_elements, other._elements) && _rows == other._rows;

        public override bool Equals(object obj) => !ReferenceEquals(null, obj) &&
                   (ReferenceEquals(this, obj) || (obj.GetType() == GetType() && Equals((Matrix)obj)));

        public override int GetHashCode()
        {
            var num = _columns;
            num = (num*397 ^ ((_elements != null) ? _elements.GetHashCode() : 0));
            return num*397 ^ _rows;
        }

        protected void AddRowTimesScalar(int row1, int row2, double scalar)
        {
            var row3 = GetRow(row2);
            for (var i = 0; i < Columns; i++)
            {
                int column;
                this[row1, column = i] = this[row1, column] + scalar*row3[i, 0];
            }
        }

        public Matrix Augment(Matrix mat)
        {
            if (Rows != mat.Rows)
            {
                throw new MatrixException("Cannot augment matrices with different number of rows");
            }
            var matrix = new Matrix(Rows, Columns + mat.Columns);
            for (var i = 0; i < Columns; i++)
            {
                matrix.SetColumn(i, GetColumn(i));
            }
            for (var j = 0; j < mat.Columns; j++)
            {
                matrix.SetColumn(j + Columns, mat.GetColumn(j));
            }
            return matrix;
        }

        public double ConditionNumber()
        {
            var sVD = new SVD(this);
            return sVD.ConditionNumber;
        }

        public Vector GetColumn(int column)
        {
            var vector = new Vector(Rows);
            for (var i = 0; i < Rows; i++)
            {
                vector[i] = this[i, column];
            }
            return vector;
        }

        public Vector GetRow(int row)
        {
            var vector = new Vector(Columns);
            for (var i = 0; i < Columns; i++)
            {
                vector[i] = this[row, i];
            }
            return vector;
        }

        private bool IsColumnZeroBelowRow(int column, int row)
        {
            bool result;
            for (var i = row; i < Rows; i++)
            {
                if (Math.Abs(this[i, column]) > 1E-08)
                {
                    result = false;
                    return result;
                }
            }
            result = true;
            return result;
        }

        protected bool IsNaN()
        {
            bool result;
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    if (!double.IsNaN(this[i, j]))
                    {
                        result = false;
                        return result;
                    }
                }
            }
            result = true;
            return result;
        }

        protected bool IsRowZero(int row)
        {
            bool result;
            for (var i = 0; i < Columns; i++)
            {
                if (Math.Abs(this[row, i]) > 1E-08)
                {
                    result = false;
                    return result;
                }
            }
            result = true;
            return result;
        }

        public double MakeRowEchelon()
        {
            var num = 1.0;
            double result;
            for (var i = 0; i < Rows; i++)
            {
                var num2 = -1;
                for (var j = 0; j < Columns; j++)
                {
                    if (!IsColumnZeroBelowRow(j, i))
                    {
                        num2 = j;
                        break;
                    }
                }
                if (num2 == -1)
                {
                    result = num;
                    return result;
                }
                var num3 = double.NaN;
                for (var k = i; k < Rows; k++)
                {
                    if (Math.Abs(this[k, num2]) > 1E-05)
                    {
                        if (i != k)
                        {
                            SwapRows(i, k);
                            num = -num;
                        }
                        num3 = this[i, num2];
                        break;
                    }
                }
                if (Math.Abs(num3 - 1.0) > 0.0001)
                {
                    MultiplyRow(i, 1.0/num3);
                    num *= num3;
                }
                while (true)
                {
                    var num4 = -1;
                    var num5 = double.NaN;
                    for (var l = i + 1; l < Rows; l++)
                    {
                        if (Math.Abs(this[l, num2] - 0.0) > 0.0001)
                        {
                            num4 = l;
                            num5 = this[l, num2];
                            break;
                        }
                    }
                    var num6 = num4;
                    if (num6 == -1)
                    {
                        break;
                    }
                    AddRowTimesScalar(num4, i, -num5);
                }
            }
            result = num;
            return result;
        }

        protected void MultiplyRow(int row, double scalar)
        {
            for (var i = 0; i < Columns; i++)
            {
                int column;
                this[row, column = i] = this[row, column]*scalar;
            }
        }

        protected static Matrix NaN(int rows, int columns)
        {
            var matrix = new Matrix(rows, columns);
            matrix.SetSize(rows, columns);
            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    matrix[i, j] = double.NaN;
                }
            }
            return matrix;
        }

        public static Matrix operator +(Matrix lhs, Matrix rhs)
        {
            if (lhs.Rows != rhs.Rows || lhs.Columns != rhs.Columns)
            {
                throw new MatrixException("Matrices are not the same size");
            }
            var matrix = new Matrix(lhs.Rows, lhs.Columns);
            for (var i = 0; i < lhs.Rows; i++)
            {
                for (var j = 0; j < lhs.Columns; j++)
                {
                    matrix[i, j] = lhs[i, j] + rhs[i, j];
                }
            }
            return matrix;
        }

        public static Matrix operator +(Matrix mat, double scalar)
        {
            var matrix = new Matrix(mat);
            for (var i = 0; i < matrix.Rows; i++)
            {
                for (var j = 0; j < matrix.Columns; j++)
                {
                    Matrix matrix2;
                    int row;
                    int column;
                    (matrix2 = matrix)[row = i, column = j] = matrix2[row, column] + scalar;
                }
            }
            return matrix;
        }

        public static Matrix operator +(double scalar, Matrix mat)
        {
            return new Matrix();
        }

        public static Matrix operator /(Matrix mat, double scalar)
        {
            if (Math.Abs(scalar - 0.0) < 0.0001)
            {
                throw new DivideByZeroException();
            }
            return mat*(1.0/scalar);
        }

        public static bool operator ==(Matrix m1, Matrix m2)
        {
            bool result;
            try
            {
                if (m2 != null && m1 != null && (m1.Rows != m2.Rows || m1.Columns != m2.Columns))
                {
                    result = false;
                }
                else
                {
                    Debug.Assert(m1 != null, "m1 != null");
                    for (var i = 0; i < m1.Rows; i++)
                    {
                        for (var j = 0; j < m1.Columns; j++)
                        {
                            if (m2 != null && Math.Abs(m1[i, j] - m2[i, j]) > 0.0001)
                            {
                                result = false;
                                return result;
                            }
                        }
                    }
                    result = true;
                }
            }
            catch (NullReferenceException)
            {
                result = false;
            }
            return result;
        }

        public static bool operator !=(Matrix m1, Matrix m2)
        {
            return !(m1 == m2);
        }

        public static Matrix operator *(Matrix lhs, Matrix rhs)
        {
            if (lhs.Columns != rhs.Rows)
            {
                throw new MatrixException("Matrices are not compatible for multiplication");
            }
            var matrix = new Matrix(lhs.Rows, rhs.Columns);
            for (var i = 0; i < matrix.Rows; i++)
            {
                for (var j = 0; j < matrix.Columns; j++)
                {
                    var num = 0.0;
                    for (var k = 0; k < lhs.Columns; k++)
                    {
                        num += lhs[i, k]*rhs[k, j];
                    }
                    matrix[i, j] = num;
                }
            }
            return matrix;
        }

        public static Matrix operator *(Matrix mat, double scalar)
        {
            var matrix = new Matrix(mat.Rows, mat.Columns);
            for (var i = 0; i < mat.Rows; i++)
            {
                for (var j = 0; j < mat.Columns; j++)
                {
                    matrix[i, j] = mat[i, j]*scalar;
                }
            }
            return matrix;
        }

        public static Matrix operator *(double scalar, Matrix mat)
        {
            return mat*scalar;
        }

        public static Matrix operator -(Matrix lhs, Matrix rhs)
        {
            if (lhs.Rows != rhs.Rows || lhs.Columns != rhs.Columns)
            {
                throw new MatrixException("Matrices are not the same size");
            }
            var matrix = new Matrix(lhs.Rows, lhs.Columns);
            for (var i = 0; i < lhs.Rows; i++)
            {
                for (var j = 0; j < lhs.Columns; j++)
                {
                    matrix[i, j] = lhs[i, j] - rhs[i, j];
                }
            }
            return matrix;
        }

        public static Matrix operator -(Matrix mat, double scalar)
        {
            var matrix = new Matrix(mat);
            for (var i = 0; i < matrix.Rows; i++)
            {
                for (var j = 0; j < matrix.Columns; j++)
                {
                    Matrix matrix2;
                    int row;
                    int column;
                    (matrix2 = matrix)[row = i, column = j] = matrix2[row, column] - scalar;
                }
            }
            return matrix;
        }

        public static Matrix operator -(Matrix mat)
        {
            var matrix = new Matrix(mat);
            for (var i = 0; i < mat.Rows; i++)
            {
                for (var j = 0; j < mat.Columns; j++)
                {
                    matrix[i, j] = -mat[i, j];
                }
            }
            return matrix;
        }

        public Matrix PseudoInverse()
        {
            var sVD = new SVD(this);
            Matrix matrix = new SquareMatrix(Columns);
            for (var i = 0; i < sVD.W.Rows; i++)
            {
                if (sVD.W[i] > 1E-05)
                {
                    matrix[i, i] = 1.0/sVD.W[i];
                }
            }
            return sVD.V*matrix*sVD.U.Transpose();
        }

        public void SetColumn(int column, Vector vec)
        {
            for (var i = 0; i < Rows; i++)
            {
                this[i, column] = vec[i];
            }
        }

        public void SetRow(int row, Vector vec)
        {
            for (var i = 0; i < Columns; i++)
            {
                this[row, i] = vec[i];
            }
        }

        protected void SetSize(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
            _elements = new double[rows, columns];
        }

        private void SwapRows(int row1, int row2)
        {
            var row3 = GetRow(row1);
            var row4 = GetRow(row2);
            SetRow(row1, row4);
            SetRow(row2, row3);
        }

        public override string ToString() => ToString(null, null);

        public Matrix Transpose()
        {
            var matrix = new Matrix(Columns, Rows);
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    matrix[j, i] = this[i, j];
                }
            }
            return matrix;
        }
    }
}