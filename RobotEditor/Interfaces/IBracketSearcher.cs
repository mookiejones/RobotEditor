using ICSharpCode.AvalonEdit.Document;
using RobotEditor.Classes;

namespace RobotEditor.Interfaces
{
    public interface IBracketSearcher
    {
        BracketSearchResult SearchBracket(TextDocument document, int offset);
    }
}