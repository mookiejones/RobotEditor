using RobotEditor.Controls.AngleConverter.Classes;
using RobotEditor.Controls.AngleConverter.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RobotEditor.Controls.AngleConverter
{
    [Localizable(false)]
    public sealed class Sphere3D : IGeometricElement3D
    {
        public Sphere3D()
        {
            Origin = new Point3D();
            Radius = 0.0;
        }

        public Sphere3D(Sphere3D sphere)
        {
            Origin = sphere.Origin;
            Radius = sphere.Radius;
        }

        public Sphere3D(Point3D origin, double radius)
        {
            Origin = origin;
            Radius = radius;
        }

        public Point3D Origin { get; set; }
        public double Radius { get; set; }

        public TransformationMatrix3D Position => new TransformationMatrix3D((Vector3D)Origin, RotationMatrix3D.Identity());

        public string ToString(string format, IFormatProvider formatProvider = null) => string.Format("Sphere3D: Centre {0:F2} Radius {1:F2}", Origin, Radius);

        public static Sphere3D FitToPoints(Collection<Point3D> points)
        {
            LeastSquaresFit3D leastSquaresFit3D = new LeastSquaresFit3D();
            return leastSquaresFit3D.FitSphereToPoints(points);
        }

        public override string ToString() => ToString("", null);
    }
}