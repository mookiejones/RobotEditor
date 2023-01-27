using RobotEditor.Controls.AngleConverter.Classes;
using RobotEditor.Controls.AngleConverter.Exceptions;
using System;

namespace RobotEditor.Controls.AngleConverter
{
    public sealed class NRSolver
    {
        private const int MaxIterations = 20;
        private const double StopCondition = 1E-07;

        public NRSolver(int numEquations, int numVariables)
        {
            NumEquations = numEquations;
            NumVariables = numVariables;
        }

        public int NumEquations { get; private set; }
        private int NumVariables { get; set; }
        private int NumStepsToConverge { get; set; }

        private Matrix CalculateJacobian(ErrorFunction errorFunction, Vector guess)
        {
            Matrix matrix = new Matrix(NumEquations, NumVariables);
            for (int i = 0; i < matrix.Columns; i++)
            {
                double num = Math.Abs(guess[i]) >= 1.0 ? Math.Abs(guess[i]) * 1E-07 : 1E-07;
                Vector vector = new Vector(guess);
                Vector vector2;
                int index;
                (vector2 = vector)[index = i] = vector2[index] + num;
                Vector v = errorFunction(vector);
                Vector vector3;
                int index2;
                (vector3 = vector)[index2 = i] = vector3[index2] - (2.0 * num);
                Vector v2 = errorFunction(vector);
                Vector vec = v - v2;
                matrix.SetColumn(i, vec / (2.0 * num));
            }
            return matrix;
        }

        private static bool IsDone(Vector delta)
        {
            bool result;
            for (int i = 0; i < delta.Rows; i++)
            {
                if (Math.Abs(delta[i]) > 1E-07)
                {
                    result = false;
                    return result;
                }
            }
            result = true;
            return result;
        }

        public Vector Solve(ErrorFunction errorFunction, Vector initialGuess)
        {
            if (initialGuess.Size != NumVariables)
            {
                throw new MatrixException("Size of the initial guess vector is not correct");
            }
            Vector vector = new Vector(initialGuess);
            NumStepsToConverge = 0;
            Vector result;
            for (int i = 0; i < 20; i++)
            {
                Matrix matrix = CalculateJacobian(errorFunction, vector);
                Vector vec = errorFunction(vector);
                Matrix matrix2 = matrix.Transpose();
                SquareMatrix squareMatrix = new SquareMatrix(matrix2 * matrix);
                Vector vec2 = matrix2 * vec;
                Vector vector2 = squareMatrix.PseudoInverse() * vec2;
                vector -= vector2;
                if (IsDone(vector2))
                {
                    NumStepsToConverge = i + 1;
                    result = vector;
                    return result;
                }
            }
            result = vector;
            return result;
        }
    }
}