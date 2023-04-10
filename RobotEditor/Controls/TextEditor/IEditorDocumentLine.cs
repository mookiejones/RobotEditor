using ICSharpCode.AvalonEdit.Document;

namespace RobotEditor.Interfaces;

public interface IEditorDocumentLine : IDocumentLine
{
    string Text { get; }
}