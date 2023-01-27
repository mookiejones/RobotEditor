using System;
using RobotEditor.Controls.AngleConverter.Classes;
using RobotEditor.Controls.AngleConverter.Interfaces;

namespace RobotEditor.Controls.AngleConverter
{
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
            var num3 = vector3D.Length() * vector3D.Length();
            var num4 = Math.Abs(1.0 - num * num);
            double num7;
            double num8;
            double d;
            if (num4 > 1E-05)
            {
                var num5 = Vector.Dot(-vector3D, direction2);
                var num6 = 1.0 / num4;
                num7 = (num * num5 - num2) * num6;
                num8 = (num * num2 - num5) * num6;
                d = num7 * (num7 + num * num8 + 2.0 * num2) + num8 * (num * num7 + num8 + 2.0 * num5) + num3;
            }
            else
            {
                num7 = -num2;
                num8 = 0.0;
                d = num2 * num7 + num3;
            }
            closestPoint1 = origin + new Vector3D(num7 * direction);
            closestPoint2 = origin2 + new Vector3D(num8 * direction2);
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