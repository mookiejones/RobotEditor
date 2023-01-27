using System;

namespace RobotEditor.Classes
{
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
}