using RobotEditor.Controls.AngleConverter.Classes;
using System;

namespace RobotEditor.Controls.AngleConverter
{
    public static class Project3D
    {
        private const double EPSILON = 0.001;

        public static Point3D PointOntoCircle(Circle3D circle, Point3D point)
        {
            Plane3D plane = new Plane3D(circle.Origin, circle.Normal);
            Point3D point3D = PointOntoPlane(plane, point);
            _ = Distance3D.Between(point, point3D);
            Vector3D vector3D = circle.Origin - point3D;
            vector3D.Normalise();
            return new Point3D();
        }

        public static Point3D PointOntoLine(Line3D line, Point3D point)
        {
            Vector3D vector3D = line.Origin - point;
            Point3D result;
            if (Math.Abs(vector3D.Length() - 0.0) < 0.001)
            {
                result = new Point3D(point);
            }
            else
            {
                double scalar = Vector.Dot(vector3D, line.Direction);
                result = line.Origin + (Vector3D)(Point3D)(line.Direction * scalar);
            }
            return result;
        }

        public static Point3D PointOntoPlane(Plane3D plane, Point3D point)
        {
            Vector3D vec = plane.Point - point;
            Vector3D vector3D = Vector3D.Cross(Vector3D.Cross(vec, plane.Normal), plane.Normal).Normalised();
            return plane.Point + (Vector3D)(Point3D)(vector3D * Vector.Dot(vec, vector3D));
        }

        public static Point3D PointOntoSphere(Sphere3D sphere, Point3D point)
        {
            Vector3D vector3D = point - sphere.Origin;
            vector3D.Normalise();
            return new Point3D();
        }
    }
}