using RobotEditor.Controls.AngleConverter.Classes;
using System;

namespace RobotEditor.Controls.AngleConverter.Interfaces
{
    public interface IGeometricElement3D : IFormattable
    {
        TransformationMatrix3D Position { get; }
    }
}