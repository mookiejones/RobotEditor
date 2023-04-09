using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using System;
using System.ComponentModel;

namespace RobotEditor.Controls.TextEditor.Indentation
{
    [Localizable(false)]
    public class IndentationStrategy : DefaultIndentationStrategy
    {
        private string _indentationString = "\t";

        public IndentationStrategy()
        {
        }

        public IndentationStrategy(TextEditorOptions options)
        {
            IndentationString = options.IndentationString;
        }

        public string IndentationString
        {
            get => _indentationString;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Indentation string must not be null or empty");
                }
                _indentationString = value;
            }
        }

        public void Indent(IDocumentAccessor document, bool keepEmptyLines)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            IndentationSettings set = new()
            {
                IndentString = IndentationString,
                LeaveEmptyLines = keepEmptyLines
            };
            IndentationClass indentationClass = new();
            indentationClass.Reformat(document, set);
        }

        public void UnIndentLine(TextDocument document, DocumentLine line)
        {
        }

        public override void IndentLine(TextDocument document, DocumentLine line)
        {
            int lineNumber = line.LineNumber;
            TextDocumentAccessor textDocumentAccessor = new(document, lineNumber, lineNumber);
            Indent(textDocumentAccessor, false);
            string text = textDocumentAccessor.Text;
            if (text.Length == 0)
            {
                base.IndentLine(document, line);
            }
        }

        public override void IndentLines(TextDocument document, int beginLine, int endLine) => Indent(new TextDocumentAccessor(document, beginLine, endLine), true);
    }
}