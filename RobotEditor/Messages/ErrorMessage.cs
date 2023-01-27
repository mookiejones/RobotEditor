using System;
using RobotEditor.Enums;

namespace RobotEditor.Messages
{
    public sealed class ErrorMessage : MessageBase
    {
        public ErrorMessage(string title, Exception ex) : base(title, ex.ToString(), MessageType.Error)
        {
        }

        public ErrorMessage(string title, Exception exception, MessageType icon)
            : base(title, exception.ToString(), icon)
        {
            Exception = exception;
        }

        public ErrorMessage(string title, string exception, MessageType icon, bool force = false)
            : base(title, exception, icon, force)
        {
        }

        public Exception Exception { get; set; }
    }
}