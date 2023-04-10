using ICSharpCode.AvalonEdit.Document;
using RobotEditor.Interfaces;
using System;
using System.IO;

namespace RobotEditor.Controls.TextEditor;

public class TextSourceAdapter : ITextBuffer
{
    private readonly ITextSource TextSource;

    protected TextSourceAdapter(ITextSource textSource)
    {
        TextSource = textSource ?? throw new ArgumentNullException(nameof(textSource));
    }

    public event EventHandler TextChanged
    {
        add { }
        remove { }
    }

    public virtual ITextBufferVersion Version => null;

    public int TextLength => TextSource.TextLength;

    public string Text => TextSource.Text;

    public virtual ITextBuffer CreateSnapshot() => new TextSourceAdapter(TextSource.CreateSnapshot());

    public ITextBuffer CreateSnapshot(int offset, int length) => new TextSourceAdapter(TextSource.CreateSnapshot(offset, length));

    public TextReader CreateReader() => TextSource.CreateReader();

    public TextReader CreateReader(int offset, int length) => TextSource.CreateSnapshot(offset, length).CreateReader();

    public char GetCharAt(int offset) => TextSource.GetCharAt(offset);

    public string GetText(int offset, int length) => TextSource.GetText(offset, length);
}