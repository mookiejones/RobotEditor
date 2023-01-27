using System;
using RobotEditor.Controls.AngleConverter.Classes;

namespace RobotEditor.Classes
{
    public static class Project3D
    {
        private const double EPSILON = 0.001;

        public static Point3D PointOntoCircle(Circle3D circle, Point3D point)
        {
            var plane = new Plane3D(circle.Origin, circle.Normal);
            var point3D = PointOntoPlane(plane, point);
            Distance3D.Between(point, point3D);
            var vector3D = circle.Origin - point3D;
            vector3D.Normalise();
            return new Point3D();
        }

        public static Point3D PointOntoLine(Line3D line, Point3D point)
        {
            var vector3D = line.Origin - point;
            Point3D result;
            if (Math.Abs(vector3D.Length() - 0.0) < 0.001)
            {
                result = new Point3D(point);
            }
            else
            {
                var scalar = Vector.Dot(vector3D, line.Direction);
                result = line.Origin + (Vector3D) ((Point3D) (line.Direction*scalar));
            }
            return result;
        }

        public static Point3D PointOntoPlane(Plane3D plane, Point3D point)
        {
            var vec = plane.Point - point;
            var vector3D = Vector3D.Cross(Vector3D.Cross(vec, plane.Normal), plane.Normal).Normalised();
            return plane.Point + (Vector3D) ((Point3D) (vector3D*Vector.Dot(vec, vector3D)));
        }

        public static Point3D PointOntoSphere(Sphere3D sphere, Point3D point)
        {
            var vector3D = point - sphere.Origin;
            vector3D.Normalise();
            return new Point3D();
        }
    }
}