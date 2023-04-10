using System;

namespace RobotEditor.Controls.AngleConverter;

[Serializable]
public sealed class MatrixNullReference : NullReferenceException
{
    public MatrixNullReference(string message)
        : base(message)
    {
    }

    public MatrixNullReference()
    {
    }
}