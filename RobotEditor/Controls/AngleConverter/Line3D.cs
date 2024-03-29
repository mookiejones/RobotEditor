using RobotEditor.Controls.AngleConverter.Classes;
using RobotEditor.Controls.AngleConverter.Interfaces;
using System;
using System.ComponentModel;

namespace RobotEditor.Controls.AngleConverter;

[Localizable(false)]
public sealed class Line3D : IGeometricElement3D, IFormattable
{
    public Line3D(Point3D origin, Vector3D direction)
    {
        Origin = origin;
        Direction = direction;
        Direction.Normalise();
    }

    public Vector3D Direction { get; private set; }

    public Point3D Origin { get; private set; }

    TransformationMatrix3D IGeometricElement3D.Position => throw new NotImplementedException();

    public string ToString(string format, IFormatProvider formatProvider) => string.Format("Line: Origin={0}, Direction={1}", Origin.ToString(format, formatProvider),
            Direction.ToString(format, formatProvider));

    public Point3D GetPoint(double u)
    {
        Vector3D vec = new(u * Direction);
        return Origin + vec;
    }

    public override string ToString() => string.Format("Line: Origin={0}, Direction={1}", Origin, Direction);
}