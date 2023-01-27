using System;
using RobotEditor.Controls.AngleConverter.Classes;

namespace RobotEditor.Controls.AngleConverter.Interfaces
{
    public interface IGeometricElement3D : IFormattable
    {
        TransformationMatrix3D Position { get; }
    }
}