namespace RobotEditor.Controls.TextEditor.Snippets.CompletionData;

public abstract class CompletionContext
{
    public ITextEditor Editor { get; set; }
    private int StartOffset { get; set; }
    private int EndOffset { get; set; }

    public int Length => EndOffset - StartOffset;

    public char CompletionChar { get; set; }
    public bool CompletionCharHandled { get; set; }
}