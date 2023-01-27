using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using RobotEditor.Classes;
using RobotEditor.Controls.AngleConverter.Classes;
using RobotEditor.Controls.AngleConverter.Interfaces;

namespace RobotEditor.Controls.AngleConverter
{
    [Localizable(false)]
    public sealed class Circle3D : IGeometricElement3D
    {
        public Circle3D()
        {
            Origin = new Point3D();
            Normal = new Vector3D();
            Radius = 0.0;
        }

        public Vector3D Normal { get; set; }
        public Point3D Origin { get; set; }

        public double Radius { get; set; }

        public TransformationMatrix3D Position => new TransformationMatrix3D((Vector3D)Origin, RotationMatrix3D.Identity());

        public string ToString(string format, IFormatProvider formatProvider) => string.Format("Circle3D: Centre {0}, Normal {1}, Radius {2:F2}", Origin, Normal, Radius);

        public static Circle3D FitToPoints(Collection<Point3D> points)
        {
            var leastSquaresFit3D = new LeastSquaresFit3D();
            return leastSquaresFit3D.FitCircleToPoints(points);
        }

        public static Circle3D FitToPoints2(Collection<Point3D> points)
        {
            var leastSquaresFit3D = new LeastSquaresFit3D();
            return leastSquaresFit3D.FitCircleToPoints2(points);
        }

        public override string ToString() => ToString(null, null);
    }
}