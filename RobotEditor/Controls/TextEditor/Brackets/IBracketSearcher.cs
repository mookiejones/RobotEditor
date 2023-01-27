using ICSharpCode.AvalonEdit.Document;

namespace RobotEditor.Controls.TextEditor.Brackets
{
    public interface IBracketSearcher
    {
        BracketSearchResult SearchBracket(TextDocument document, int offset);
    }
}