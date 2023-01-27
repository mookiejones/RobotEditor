using System.ComponentModel;

namespace RobotEditor.Controls.TextEditor.Indentation
{
    internal sealed class IndentationSettings
    {
        [Localizable(false)] public string IndentString = "\t";
        public bool LeaveEmptyLines = true;
    }
}