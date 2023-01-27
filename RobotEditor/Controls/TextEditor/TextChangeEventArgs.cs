using System;

namespace RobotEditor.Controls.TextEditor
{
    public abstract class TextChangeEventArgs : EventArgs
    {
        protected TextChangeEventArgs(int offset, string removedText, string insertedText)
        {
            Offset = offset;
            RemovedText = removedText ?? string.Empty;
            InsertedText = insertedText ?? string.Empty;
        }

        private int Offset { get; set; }
        private string RemovedText { get; set; }
        private string InsertedText { get; set; }
    }
}