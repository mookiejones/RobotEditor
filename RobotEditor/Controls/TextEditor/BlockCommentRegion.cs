namespace RobotEditor.Controls.TextEditor;

public sealed class BlockCommentRegion
{
    public BlockCommentRegion(string commentStart, string commentEnd, int startOffset, int endOffset)
    {
        CommentStart = commentStart;
        CommentEnd = commentEnd;
        StartOffset = startOffset;
        EndOffset = endOffset;
    }

    public string CommentStart { get; private set; }
    public string CommentEnd { get; private set; }
    public int StartOffset { get; private set; }
    public int EndOffset { get; private set; }

    public override int GetHashCode()
    {
        int num = 0;
        if (CommentStart != null)
        {
            num += 1000000007 * CommentStart.GetHashCode();
        }
        if (CommentEnd != null)
        {
            num += 1000000009 * CommentEnd.GetHashCode();
        }
        num += 1000000021 * StartOffset.GetHashCode();
        return num + (1000000033 * EndOffset.GetHashCode());
    }

    public override bool Equals(object obj) => obj is BlockCommentRegion blockCommentRegion &&
               CommentStart == blockCommentRegion.CommentStart && CommentEnd == blockCommentRegion.CommentEnd &&
                StartOffset == blockCommentRegion.StartOffset && EndOffset == blockCommentRegion.EndOffset;
}