using RobotEditor.Controls.AngleConverter.Exceptions;
using System;

namespace RobotEditor.Controls.AngleConverter.Classes
{
    public class Vector : Matrix
    {
        protected Vector()
            : base(0, 0)
        {
        }

        public Vector(Matrix mat)
        {
            if (mat.Rows != 1 && mat.Columns != 1)
            {
                throw new MatrixException("Cannot convert matrix to Vector. It has more than one row or column");
            }
            if (mat.Columns == 1)
            {
                mat = mat.Transpose();
            }
            if (mat.Rows == 1)
            {
                Size = mat.Columns;
                for (int i = 0; i < mat.Columns; i++)
                {
                    this[i] = mat[0, i];
                }
            }
        }

        public Vector(int size)
            : base(size, 1)
        {
        }

        public Vector(int size, params double[] elements)
            : base(size, 1, elements)
        {
            if (elements.Length != size)
            {
                throw new MatrixException("Number of elements does not match vector dimension");
            }
        }

        public double this[int index]
        {
            get => base[index, 0];
            set => base[index, 0] = value;
        }

        public int Size
        {
            get => Rows;
            set => SetSize(value, 1);
        }

        public override bool Equals(object obj) => base.Equals(obj);

        public override int GetHashCode() => base.GetHashCode();

        public static double Dot(Vector vec1, Vector vec2)
        {
            if (vec1.Size != vec2.Size)
            {
                throw new MatrixException("Vectors are not of equal size");
            }
            Matrix matrix = vec1.Transpose() * vec2;
            return matrix[0, 0];
        }

        public double Length()
        {
            double num = 0.0;
            for (int i = 0; i < Size; i++)
            {
                num += this[i] * this[i];
            }
            return Math.Sqrt(num);
        }

        public void Normalise()
        {
            double num = Length();
            for (int i = 0; i < Size; i++)
            {
                int index;
                this[index = i] = this[index] / num;
            }
        }

        public Vector Normalised() => new(this / Length());

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1 + v2);
        }

        public static Vector operator +(Vector vec, double scalar)
        {
            return new Vector(vec + scalar);
        }

        public static Vector Add(Vector vec, double scalar) => new(vec + scalar);

        public static Vector operator +(double scalar, Vector vec)
        {
            return new Vector(vec + scalar);
        }

        public static Vector operator /(Vector vec, double scalar)
        {
            return new Vector(vec / scalar);
        }

        public static Vector Divide(Vector vec, double scalar) => new(vec / scalar);

        public static Vector operator *(Matrix mat, Vector vec)
        {
            return new Vector(mat * vec);
        }

        public static Vector operator *(Vector vec, double scalar)
        {
            return new Vector(vec * scalar);
        }

        public static Vector Multiply(Vector vec, double scalar) => new(vec * scalar);

        public static Vector operator *(double scalar, Vector vec)
        {
            return new Vector(scalar * vec);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1 - v2);
        }

        public static Vector Subtract(Vector v1, Vector v2) => new(v1 - v2);

        public static bool operator ==(Vector v1, Vector v2)
        {
            return v1 == v2;
        }

        public static bool Equals(Vector v1, Vector v2) => v1 == v2;

        public static bool operator !=(Vector v1, Vector v2)
        {
            return !(v1 == v2);
        }

        public static Vector operator -(Vector vec, double scalar)
        {
            return new Vector(vec - scalar);
        }

        public static Vector operator -(Vector v)
        {
            return new Vector(-v);
        }

        public static Vector Negate(Vector v) => new(-v);
    }
}