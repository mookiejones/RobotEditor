using ICSharpCode.AvalonEdit.Document;

namespace RobotEditor.Controls.TextEditor;

public sealed class StringTextBuffer : TextSourceAdapter
{
    public StringTextBuffer(string text)
        : base(new StringTextSource(text))
    {
    }
}