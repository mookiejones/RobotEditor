using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using RobotEditor.Controls.AngleConverter;
using RobotEditor.Controls.AngleConverter.Classes;
using RobotEditor.Controls.AngleConverter.Exceptions;
using RobotEditor.Controls.AngleConverter.Interfaces;

namespace RobotEditor.Classes
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
                var num = Errors.Sum((double num2) => num2*num2);
                num /= Errors.Count;
                return Math.Sqrt(num);
            }
        }

        private double StandardDeviationError { get; set; }
        private TransformationMatrix3D Transform { get; set; }

        private void CalculateErrors(IList<Point3D> points, IGeometricElement3D element)
        {
            Errors = new Collection<double>();
            var num = 0.0;
            var num2 = 0.0;
            MaxError = 0.0;
            PointWithLargestError = -1;
            for (var i = 0; i < points.Count; i++)
            {
                var e = points[i];
                var num3 = Distance3D.Between(element, e);
                Errors.Add(num3);
                num += num3;
                num2 += num3*num3;
                if (num3 > MaxError)
                {
                    MaxError = num3;
                    PointWithLargestError = i;
                }
            }
            var count = points.Count;
            AverageError = num/count;
            StandardDeviationError = Math.Sqrt(count*num2 - AverageError*AverageError)/count;
        }

        public Point3D Centroid(Collection<Point3D> points)
        {
            var count = points.Count;
            var num = 0.0;
            var num2 = 0.0;
            var num3 = 0.0;
            foreach (var current in points)
            {
                num += current.X;
                num2 += current.Y;
                num3 += current.Z;
            }
            var point3D = new Point3D(num/count, num2/count, num3/count);
            Transform = new TransformationMatrix3D(new Vector3D(point3D.X, point3D.Y, point3D.Z), new RotationMatrix3D());
            CalculateErrors(points, point3D);
            return point3D;
        }

        private Vector Circle3DErrorFunction(Vector vec)
        {
            var vector = new Vector(_solver.NumEquations);
            var index = 0;
            var point3D = new Point3D(vec[0], vec[1], vec[2]);
            var plane = new Plane3D(point3D, new Vector3D(vec[3], vec[4], vec[5]));
            foreach (var current in _measuredPoints)
            {
                var p = Project3D.PointOntoPlane(plane, current);
                var vector3D = point3D - p;
                vector3D.Normalise();
                var point3D2 = new Point3D();
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
            _solver = new NRSolver(points.Count*3 + 1, 7);
            _measuredPoints = points;
            var leastSquaresFit3D = new LeastSquaresFit3D();
            var plane3D = leastSquaresFit3D.FitPlaneToPoints(points);
            var initialGuess = new Circle3D
            {
                Origin = leastSquaresFit3D.Centroid(points),
                Normal = plane3D.Normal,
                Radius = leastSquaresFit3D.AverageError
            };
            var vector = _solver.Solve(Circle3DErrorFunction, VectorFromCircle3D(initialGuess));
            var circle3D = new Circle3D
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
            var circle3D = new Circle3D();
            var leastSquaresFit3D = new LeastSquaresFit3D();
            var matrix = new Matrix(points.Count, 7);
            var vector = new Vector(points.Count);
            for (var i = 0; i < 50; i++)
            {
                circle3D.Origin = leastSquaresFit3D.Centroid(points);
                circle3D.Radius = leastSquaresFit3D.RmsError;
                var plane3D = leastSquaresFit3D.FitPlaneToPoints(points);
                circle3D.Normal = plane3D.Normal;
                var num = 0;
                foreach (var current in points)
                {
                    var p = Project3D.PointOntoPlane(plane3D, current);
                    var vector3D = circle3D.Origin - p;
                    vector3D.Normalise();
                    matrix[num, 0] = vector3D[0];
                    matrix[num, 1] = vector3D[1];
                    matrix[num, 2] = vector3D[2];
                    matrix[num, 3] = plane3D.Normal.X;
                    matrix[num, 4] = plane3D.Normal.Y;
                    matrix[num, 5] = plane3D.Normal.Z;
                    matrix[num, 6] = -1.0;
                    var value = Distance3D.PointToCircle(circle3D, current);
                    vector[num] = value;
                    num++;
                }
                var vector2 = matrix.PseudoInverse()*vector;
                if (vector2.Length() < 1E-06)
                {
                    break;
                }
                var origin = circle3D.Origin;
                origin.X += vector2[0];
                var origin2 = circle3D.Origin;
                origin2.Y += vector2[1];
                var origin3 = circle3D.Origin;
                origin3.Z += vector2[2];
                var normal = circle3D.Normal;
                normal.X += vector2[3];
                var normal2 = circle3D.Normal;
                normal2.Y += vector2[4];
                var normal3 = circle3D.Normal;
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
            var point3D = Centroid(points);
            var num = 0.0;
            var num2 = 0.0;
            var num3 = 0.0;
            var num4 = 0.0;
            var num5 = 0.0;
            var num6 = 0.0;
            foreach (var current in points)
            {
                var num7 = current.X - point3D.X;
                var num8 = current.Y - point3D.Y;
                var num9 = current.Z - point3D.Z;
                num += num7*num7;
                num2 += num8*num8;
                num3 += num9*num9;
                num4 += num7*num8;
                num5 += num8*num9;
                num6 += num7*num9;
            }
            var mat = new SquareMatrix(3, new[]
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
            var sVD = new SVD(mat);
            var direction = new Vector3D(sVD.U.GetColumn(sVD.SmallestSingularIndex));
            var line3D = new Line3D(point3D, direction);
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
            var point3D = Centroid(points);
            var num = 0.0;
            var num2 = 0.0;
            var num3 = 0.0;
            var num4 = 0.0;
            var num5 = 0.0;
            var num6 = 0.0;
            foreach (var current in points)
            {
                var num7 = current.X - point3D.X;
                var num8 = current.Y - point3D.Y;
                var num9 = current.Z - point3D.Z;
                num += num7*num7;
                num2 += num8*num8;
                num3 += num9*num9;
                num4 += num7*num8;
                num5 += num8*num9;
                num6 += num7*num9;
            }
            var mat = new SquareMatrix(3, new[]
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
            var sVD = new SVD(mat);
            var normal = new Vector3D(sVD.U.GetColumn(sVD.SmallestSingularIndex));
            var plane3D = new Plane3D(point3D, normal);
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
            var sphere3D = new Sphere3D();
            var leastSquaresFit3D = new LeastSquaresFit3D();
            sphere3D.Origin = leastSquaresFit3D.Centroid(points);
            sphere3D.Radius = leastSquaresFit3D.RmsError;
            var matrix = new Matrix(points.Count, 4);
            var vector = new Vector(points.Count);
            for (var i = 0; i < 50; i++)
            {
                var num = 0;
                foreach (var current in points)
                {
                    var vector3D = Project3D.PointOntoSphere(sphere3D, current) - sphere3D.Origin;
                    vector3D.Normalise();
                    matrix[num, 0] = vector3D.X;
                    matrix[num, 1] = vector3D.Y;
                    matrix[num, 2] = vector3D.Z;
                    matrix[num, 3] = -1.0;
                    var value = Distance3D.PointToSphere(sphere3D, current);
                    vector[num] = value;
                    num++;
                }
                var vector2 = matrix.PseudoInverse()*vector;
                if (vector2.Length() < 1E-06)
                {
                    break;
                }
                var origin = sphere3D.Origin;
                origin.X += vector2[0];
                var origin2 = sphere3D.Origin;
                origin2.Y += vector2[1];
                var origin3 = sphere3D.Origin;
                origin3.Z += vector2[2];
                sphere3D.Radius -= vector2[3];
            }
            CalculateErrors(points, sphere3D);
            return sphere3D;
        }

        private static Vector VectorFromCircle3D(Circle3D initialGuess)
        {
            var vector = new Vector(7);
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
                var num = (Math.Abs(guess[i]) >= 1.0) ? (Math.Abs(guess[i])*1E-07) : 1E-07;
                var vector = new Vector(guess);
                Vector vector2;
                int index;
                (vector2 = vector)[index = i] = vector2[index] + num;
                var v = errorFunction(vector);
                Vector vector3;
                int index2;
                (vector3 = vector)[index2 = i] = vector3[index2] - 2.0*num;
                var v2 = errorFunction(vector);
                var vec = v - v2;
                matrix.SetColumn(i, vec/(2.0*num));
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
                var squareMatrix = new SquareMatrix(matrix2*matrix);
                var vec2 = matrix2*vec;
                var vector2 = squareMatrix.PseudoInverse()*vec2;
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

    public delegate Vector ErrorFunction(Vector vec);

    public static class Distance3D
    {
        private static TYP3D getType(IGeometricElement3D geo)
        {
            TYP3D result;
            if (geo is Point3D)
            {
                result = TYP3D.Point3D;
            }
            else
            {
                if (geo is Line3D)
                {
                    result = TYP3D.Line3D;
                }
                else
                {
                    if (geo is Plane3D)
                    {
                        result = TYP3D.Plane3D;
                    }
                    else
                    {
                        if (geo is Circle3D)
                        {
                            result = TYP3D.Circle3D;
                        }
                        else
                        {
                            if (geo is Sphere3D)
                            {
                                result = TYP3D.Sphere3D;
                            }
                            else
                            {
                                result = TYP3D.None;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static double Between(IGeometricElement3D e1, IGeometricElement3D e2)
        {
            double result;
            if (e1 is Point3D)
            {
                var point3D = e1 as Point3D;
                switch (getType(e2))
                {
                    case TYP3D.Point3D:
                        result = PointToPoint(point3D, e2 as Point3D);
                        return result;
                    case TYP3D.Line3D:
                        result = PointToLine(e2 as Line3D, point3D);
                        return result;
                    case TYP3D.Plane3D:
                        result = PointToPlane(e2 as Plane3D, point3D);
                        return result;
                    case TYP3D.Circle3D:
                        result = PointToCircle(e2 as Circle3D, point3D);
                        return result;
                    case TYP3D.Sphere3D:
                        result = PointToSphere(e2 as Sphere3D, point3D);
                        return result;
                }
            }
            else
            {
                if (e1 is Line3D)
                {
                    var line3D = e1 as Line3D;
                    switch (getType(e2))
                    {
                        case TYP3D.Point3D:
                            result = PointToLine(line3D, e2 as Point3D);
                            return result;
                        case TYP3D.Line3D:
                            result = LineToLine(line3D, e2 as Line3D);
                            return result;
                        case TYP3D.Plane3D:
                        case TYP3D.Sphere3D:
                            throw new NotImplementedException();
                        case TYP3D.Circle3D:
                            result = LineToCircle(e2 as Circle3D, line3D);
                            return result;
                    }
                }
                else
                {
                    if (e1 is Plane3D)
                    {
                        var plane = e1 as Plane3D;
                        switch (getType(e2))
                        {
                            case TYP3D.Point3D:
                                result = PointToPlane(plane, e2 as Point3D);
                                return result;
                            case TYP3D.Line3D:
                            case TYP3D.Plane3D:
                            case TYP3D.Circle3D:
                            case TYP3D.Sphere3D:
                                throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        if (e1 is Circle3D)
                        {
                            var circle = e1 as Circle3D;
                            switch (getType(e2))
                            {
                                case TYP3D.Point3D:
                                    result = PointToCircle(circle, e2 as Point3D);
                                    return result;
                                case TYP3D.Line3D:
                                case TYP3D.Plane3D:
                                case TYP3D.Circle3D:
                                case TYP3D.Sphere3D:
                                    throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            if (e1 is Sphere3D)
                            {
                                var sphere = e1 as Sphere3D;
                                switch (getType(e2))
                                {
                                    case TYP3D.Point3D:
                                        result = PointToSphere(sphere, e2 as Point3D);
                                        return result;
                                    case TYP3D.Line3D:
                                    case TYP3D.Plane3D:
                                    case TYP3D.Circle3D:
                                    case TYP3D.Sphere3D:
                                        throw new NotImplementedException();
                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                        }
                    }
                }
            }
            result = -1.0;
            return result;
        }

        private static double LineToCircle(Circle3D circle, Line3D point) => throw new NotImplementedException();

        private static double LineToLine(Line3D line1, Line3D line2)
        {
            Point3D point3D;
            Point3D point3D2;
            return LineToLine(line1, line2, out point3D, out point3D2);
        }

        private static double LineToLine(Line3D line1, Line3D line2, out Point3D closestPoint1,
            out Point3D closestPoint2)
        {
            var origin = line1.Origin;
            var origin2 = line2.Origin;
            var direction = line1.Direction;
            var direction2 = line2.Direction;
            var vector3D = origin - origin2;
            var num = Vector.Dot(-direction, direction2);
            var num2 = Vector.Dot(vector3D, direction);
            var num3 = vector3D.Length()*vector3D.Length();
            var num4 = Math.Abs(1.0 - num*num);
            double num7;
            double num8;
            double d;
            if (num4 > 1E-05)
            {
                var num5 = Vector.Dot(-vector3D, direction2);
                var num6 = 1.0/num4;
                num7 = (num*num5 - num2)*num6;
                num8 = (num*num2 - num5)*num6;
                d = num7*(num7 + num*num8 + 2.0*num2) + num8*(num*num7 + num8 + 2.0*num5) + num3;
            }
            else
            {
                num7 = -num2;
                num8 = 0.0;
                d = num2*num7 + num3;
            }
            closestPoint1 = origin + new Vector3D(num7*direction);
            closestPoint2 = origin2 + new Vector3D(num8*direction2);
            return Math.Sqrt(d);
        }

        public static double PointToCircle(Circle3D circle, Point3D point)
        {
            var plane = new Plane3D(circle.Origin, circle.Normal);
            var point3D = Project3D.PointOntoPlane(plane, point);
            Between(point, point3D);
            var vector3D = circle.Origin - point3D;
            vector3D.Normalise();
            var p = new Point3D();
            return PointToPoint(point, p);
        }

        private static double PointToLine(Line3D line, Point3D point) => Vector3D.Cross(line.Direction, line.Origin - point).Length();

        private static double PointToPlane(Plane3D plane, Point3D point) => Vector.Dot(plane.Normal, point - plane.Point);

        private static double PointToPoint(Point3D p1, Point3D p2) => (p1 - p2).Length();

        public static double PointToSphere(Sphere3D sphere, Point3D point) => (point - sphere.Origin).Length() - sphere.Radius;

        private enum TYP3D
        {
            Point3D,
            Line3D,
            Plane3D,
            Circle3D,
            Sphere3D,
            None
        }
    }
}