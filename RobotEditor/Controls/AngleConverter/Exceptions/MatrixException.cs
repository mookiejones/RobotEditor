using System;
using System.ComponentModel;

namespace RobotEditor.Controls.AngleConverter.Exceptions
{
    [Serializable]
    public sealed class MatrixException : Exception
    {
        public MatrixException([Localizable(false)] string message)
            : base(message)
        {
        }
    }
}