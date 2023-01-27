using System;
using System.ComponentModel;

namespace RobotEditor.Exceptions
{
    public class ExceptionDialogShowingEventArgs : CancelEventArgs
    {
        internal ExceptionDialogShowingEventArgs(Exception exception)
        {
            this.Exception = exception;
        }

        public Exception Exception { get; }
    }
}