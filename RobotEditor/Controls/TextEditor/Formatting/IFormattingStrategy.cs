using ICSharpCode.AvalonEdit.Document;
using RobotEditor.Controls.TextEditor;

namespace RobotEditor.Interfaces;

public interface IFormattingStrategy
{
    void FormatLine(ITextEditor editor, char charTyped);
    void IndentLine(ITextEditor editor, IDocumentLine line);
    void IndentLines(ITextEditor editor, int beginLine, int endLine);
    void SurroundSelectionWithComment(ITextEditor editor);
}