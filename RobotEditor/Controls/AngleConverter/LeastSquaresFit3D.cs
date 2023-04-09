using RobotEditor.Controls.AngleConverter.Classes;
using RobotEditor.Controls.AngleConverter.Exceptions;
using RobotEditor.Controls.AngleConverter.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace RobotEditor.Controls.AngleConverter
{
    [Localizable(false)]
    public sealed class LeastSquaresFit3D
    {
        private Collection<Point3D> _measuredPoints;
        private NRSolver _solver;
        private double AverageError { get; set; }
        private Collection<double> Errors { get; set; }
        private double MaxError { get; set; }
        private int PointWithLargestError { get; set; }

        private double RmsError
        {
            get
            {
                double num = Errors.Sum((num2) => num2 * num2);
                num /= Errors.Count;
                return Math.Sqrt(num);
            }
        }

        private double StandardDeviationError { get; set; }
        private TransformationMatrix3D Transform { get; set; }

        private void CalculateErrors(IList<Point3D> points, IGeometricElement3D element)
        {
            Errors = new Collection<double>();
            double num = 0.0;
            double num2 = 0.0;
            MaxError = 0.0;
            PointWithLargestError = -1;
            for (int i = 0; i < points.Count; i++)
            {
                Point3D e = points[i];
                double num3 = Distance3D.Between(element, e);
                Errors.Add(num3);
                num += num3;
                num2 += num3 * num3;
                if (num3 > MaxError)
                {
                    MaxError = num3;
                    PointWithLargestError = i;
                }
            }
            int count = points.Count;
            AverageError = num / count;
            StandardDeviationError = Math.Sqrt((count * num2) - (AverageError * AverageError)) / count;
        }

        public Point3D Centroid(Collection<Point3D> points)
        {
            int count = points.Count;
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            foreach (Point3D current in points)
            {
                num += current.X;
                num2 += current.Y;
                num3 += current.Z;
            }
            Point3D point3D = new(num / count, num2 / count, num3 / count);
            Transform = new TransformationMatrix3D(new Vector3D(point3D.X, point3D.Y, point3D.Z), new RotationMatrix3D());
            CalculateErrors(points, point3D);
            return point3D;
        }

        private Vector Circle3DErrorFunction(Vector vec)
        {
            Vector vector = new(_solver.NumEquations);
            int index = 0;
            Point3D point3D = new(vec[0], vec[1], vec[2]);
            Plane3D plane = new(point3D, new Vector3D(vec[3], vec[4], vec[5]));
            foreach (Point3D current in _measuredPoints)
            {
                Point3D p = Project3D.PointOntoPlane(plane, current);
                Vector3D vector3D = point3D - p;
                vector3D.Normalise();
                Point3D point3D2 = new();
                vector[index++] = current.X - point3D2.X;
                vector[index++] = current.Y - point3D2.Y;
                vector[index++] = current.Z - point3D2.Z;
            }
            vector[index] = new Vector3D(vec[3], vec[4], vec[5]).Length() - 1.0;
            return vector;
        }

        public Circle3D FitCircleToPoints(Collection<Point3D> points)
        {
            if (points == null)
            {
                throw new MatrixNullReference();
            }
            if (points.Count < 3)
            {
                throw new ArgumentException("Need at least 3 points to fit circle");
            }
            _solver = new NRSolver((points.Count * 3) + 1, 7);
            _measuredPoints = points;
            LeastSquaresFit3D leastSquaresFit3D = new();
            Plane3D plane3D = leastSquaresFit3D.FitPlaneToPoints(points);
            Circle3D initialGuess = new()
            {
                Origin = leastSquaresFit3D.Centroid(points),
                Normal = plane3D.Normal,
                Radius = leastSquaresFit3D.AverageError
            };
            Vector vector = _solver.Solve(Circle3DErrorFunction, VectorFromCircle3D(initialGuess));
            Circle3D circle3D = new()
            {
                Origin = new Point3D(vector[0], vector[1], vector[2]),
                Normal = new Vector3D(vector[3], vector[4], vector[5]),
                Radius = vector[6]
            };
            CalculateErrors(points, circle3D);
            return circle3D;
        }

        public Circle3D FitCircleToPoints2(Collection<Point3D> points)
        {
            if (points == null)
            {
                throw new MatrixNullReference();
            }
            if (points.Count < 3)
            {
                throw new ArgumentException("Need at least 3 points to fit circle");
            }
            Circle3D circle3D = new();
            LeastSquaresFit3D leastSquaresFit3D = new();
            Matrix matrix = new(points.Count, 7);
            Vector vector = new(points.Count);
            for (int i = 0; i < 50; i++)
            {
                circle3D.Origin = leastSquaresFit3D.Centroid(points);
                circle3D.Radius = leastSquaresFit3D.RmsError;
                Plane3D plane3D = leastSquaresFit3D.FitPlaneToPoints(points);
                circle3D.Normal = plane3D.Normal;
                int num = 0;
                foreach (Point3D current in points)
                {
                    Point3D p = Project3D.PointOntoPlane(plane3D, current);
                    Vector3D vector3D = circle3D.Origin - p;
                    vector3D.Normalise();
                    matrix[num, 0] = vector3D[0];
                    matrix[num, 1] = vector3D[1];
                    matrix[num, 2] = vector3D[2];
                    matrix[num, 3] = plane3D.Normal.X;
                    matrix[num, 4] = plane3D.Normal.Y;
                    matrix[num, 5] = plane3D.Normal.Z;
                    matrix[num, 6] = -1.0;
                    double value = Distance3D.PointToCircle(circle3D, current);
                    vector[num] = value;
                    num++;
                }
                Vector vector2 = matrix.PseudoInverse() * vector;
                if (vector2.Length() < 1E-06)
                {
                    break;
                }
                Point3D origin = circle3D.Origin;
                origin.X += vector2[0];
                Point3D origin2 = circle3D.Origin;
                origin2.Y += vector2[1];
                Point3D origin3 = circle3D.Origin;
                origin3.Z += vector2[2];
                Vector3D normal = circle3D.Normal;
                normal.X += vector2[3];
                Vector3D normal2 = circle3D.Normal;
                normal2.Y += vector2[4];
                Vector3D normal3 = circle3D.Normal;
                normal3.Z += vector2[5];
                circle3D.Radius -= vector2[6];
            }
            CalculateErrors(points, circle3D);
            return circle3D;
        }

        public Line3D FitLineToPoints(Collection<Point3D> points)
        {
            if (points == null)
            {
                throw new MatrixNullReference();
            }
            Point3D point3D = Centroid(points);
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            foreach (Point3D current in points)
            {
                double num7 = current.X - point3D.X;
                double num8 = current.Y - point3D.Y;
                double num9 = current.Z - point3D.Z;
                num += num7 * num7;
                num2 += num8 * num8;
                num3 += num9 * num9;
                num4 += num7 * num8;
                num5 += num8 * num9;
                num6 += num7 * num9;
            }
            SquareMatrix mat = new(3, new[]
            {
                num2 + num3,
                -num4,
                -num6,
                -num4,
                num3 + num,
                -num5,
                -num6,
                -num5,
                num + num2
            });
            SVD sVD = new(mat);
            Vector3D direction = new(sVD.U.GetColumn(sVD.SmallestSingularIndex));
            Line3D line3D = new(point3D, direction);
            CalculateErrors(points, line3D);
            return line3D;
        }

        public Plane3D FitPlaneToPoints(Collection<Point3D> points)
        {
            if (points == null)
            {
                throw new NullReferenceException();
            }
            if (points.Count < 3)
            {
                throw new MatrixException("Not enough points to fit a plane");
            }
            Point3D point3D = Centroid(points);
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            foreach (Point3D current in points)
            {
                double num7 = current.X - point3D.X;
                double num8 = current.Y - point3D.Y;
                double num9 = current.Z - point3D.Z;
                num += num7 * num7;
                num2 += num8 * num8;
                num3 += num9 * num9;
                num4 += num7 * num8;
                num5 += num8 * num9;
                num6 += num7 * num9;
            }
            SquareMatrix mat = new(3, new[]
            {
                num,
                num4,
                num6,
                num4,
                num2,
                num5,
                num6,
                num5,
                num3
            });
            SVD sVD = new(mat);
            Vector3D normal = new(sVD.U.GetColumn(sVD.SmallestSingularIndex));
            Plane3D plane3D = new(point3D, normal);
            CalculateErrors(points, plane3D);
            return plane3D;
        }

        public Sphere3D FitSphereToPoints(Collection<Point3D> points)
        {
            if (points == null)
            {
                throw new NullReferenceException();
            }
            if (points.Count < 4)
            {
                throw new MatrixException("Need at least 4 points to fit sphere");
            }
            Sphere3D sphere3D = new();
            LeastSquaresFit3D leastSquaresFit3D = new();
            sphere3D.Origin = leastSquaresFit3D.Centroid(points);
            sphere3D.Radius = leastSquaresFit3D.RmsError;
            Matrix matrix = new(points.Count, 4);
            Vector vector = new(points.Count);
            for (int i = 0; i < 50; i++)
            {
                int num = 0;
                foreach (Point3D current in points)
                {
                    Vector3D vector3D = Project3D.PointOntoSphere(sphere3D, current) - sphere3D.Origin;
                    vector3D.Normalise();
                    matrix[num, 0] = vector3D.X;
                    matrix[num, 1] = vector3D.Y;
                    matrix[num, 2] = vector3D.Z;
                    matrix[num, 3] = -1.0;
                    double value = Distance3D.PointToSphere(sphere3D, current);
                    vector[num] = value;
                    num++;
                }
                Vector vector2 = matrix.PseudoInverse() * vector;
                if (vector2.Length() < 1E-06)
                {
                    break;
                }
                Point3D origin = sphere3D.Origin;
                origin.X += vector2[0];
                Point3D origin2 = sphere3D.Origin;
                origin2.Y += vector2[1];
                Point3D origin3 = sphere3D.Origin;
                origin3.Z += vector2[2];
                sphere3D.Radius -= vector2[3];
            }
            CalculateErrors(points, sphere3D);
            return sphere3D;
        }

        private static Vector VectorFromCircle3D(Circle3D initialGuess)
        {
            Vector vector = new(7);
            vector[0] = initialGuess.Origin.X;
            vector[1] = initialGuess.Origin.Y;
            vector[2] = initialGuess.Origin.Z;
            vector[3] = initialGuess.Normal.X;
            vector[4] = initialGuess.Normal.Y;
            vector[5] = initialGuess.Normal.Z;
            vector[6] = initialGuess.Radius;
            return vector;
        }
    }

    public delegate Vector ErrorFunction(Vector vec);
}