using System;
using RobotEditor.Controls.AngleConverter.Classes;
using RobotEditor.Controls.AngleConverter.Exceptions;

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
            var matrix = new Matrix(NumEquations, NumVariables);
            for (var i = 0; i < matrix.Columns; i++)
            {
                var num = Math.Abs(guess[i]) >= 1.0 ? Math.Abs(guess[i]) * 1E-07 : 1E-07;
                var vector = new Vector(guess);
                Vector vector2;
                int index;
                (vector2 = vector)[index = i] = vector2[index] + num;
                var v = errorFunction(vector);
                Vector vector3;
                int index2;
                (vector3 = vector)[index2 = i] = vector3[index2] - 2.0 * num;
                var v2 = errorFunction(vector);
                var vec = v - v2;
                matrix.SetColumn(i, vec / (2.0 * num));
            }
            return matrix;
        }

        private static bool IsDone(Vector delta)
        {
            bool result;
            for (var i = 0; i < delta.Rows; i++)
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
            var vector = new Vector(initialGuess);
            NumStepsToConverge = 0;
            Vector result;
            for (var i = 0; i < 20; i++)
            {
                var matrix = CalculateJacobian(errorFunction, vector);
                var vec = errorFunction(vector);
                var matrix2 = matrix.Transpose();
                var squareMatrix = new SquareMatrix(matrix2 * matrix);
                var vec2 = matrix2 * vec;
                var vector2 = squareMatrix.PseudoInverse() * vec2;
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