using System;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.Document;
using RobotEditor.Structs;

namespace RobotEditor.Interfaces
{
    public interface IEditor : ITextBuffer, IServiceProvider
    {
        new string Text { get; set; }
        int TotalNumberOfLines { get; }
        event EventHandler<TextChangeEventArgs> Changing;
        event EventHandler<TextChangeEventArgs> Changed;
        IEditorDocumentLine GetLine(int lineNumber);
        IEditorDocumentLine GetLineForOffset(int offset);
        int PositionToOffset(int line, int column);
        Location OffsetToPosition(int offset);

        [Localizable(false)]
        void Insert(int offset, string text);

        void Insert(int offset, string text, AnchorMovementType defaultAnchorMovementType);
        void Remove(int offset, int length);
        void Replace(int offset, int length, string newText);
        void StartUndoableAction();
        void EndUndoableAction();
        IDisposable OpenUndoGroup();
        ITextAnchor CreateAnchor(int offset);
    }
}