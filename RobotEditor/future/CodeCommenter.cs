using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using RobotEditor.Controls.TextEditor;

namespace RobotEditor.future
{
    public class CodeCommenter
    {
        private static readonly List<char> Whitespaces = new List<char>
		{
			' ',
			'\t'
		};
        private string commentMarker = ";";
        public bool AllowMultipleComments
        {
            get;
            set;
        }
        public bool CommentCarretLineIfNoSelection
        {
            get;
            set;
        }
        public string CommentMarker
        {
            get => commentMarker;
            set => commentMarker = value;
        }
        public CommentMode Mode
        {
            get;
            set;
        }
        public CodeCommenter(CommentMode mode)
        {
            Mode = mode;
            CommentCarretLineIfNoSelection = true;
        }
        public bool CommentLine(Editor kukaTextEditor, DocumentLine documentLine) => CommentLine(kukaTextEditor.Document, documentLine);
        public bool CommentLine(TextDocument document, DocumentLine documentLine)
        {
            if (Mode == CommentMode.BeginOfLine)
            {
                return CommentAtBeginOfLine(document, documentLine);
            }
            return CommentAtBeginOfText(document, documentLine);
        }
        public bool CommentLines(Editor kukaTextEditor, int startLine, int endLine)
        {
            if (endLine > kukaTextEditor.Document.LineCount)
            {
                throw new ArgumentOutOfRangeException("endLine", "End line must not be lager than the line count");
            }
            if (endLine < startLine)
            {
                throw new ArgumentException("The start line must be smaller than the end line");
            }
            var flag = false;
            for (var i = startLine; i <= endLine; i++)
            {
                var documentLine = kukaTextEditor.Document.Lines[i - 1];
                flag |= CommentLine(kukaTextEditor.Document, documentLine);
            }
            return flag;
        }
        public bool CommentLines(Editor kukaTextEditor, IEnumerable<DocumentLine> documentLines)
        {
            if (kukaTextEditor == null)
            {
                throw new ArgumentNullException("kukaTextEditor");
            }
            var flag = false;
            foreach (var current in documentLines)
            {
                flag |= CommentLine(kukaTextEditor.Document, current);
            }
            return flag;
        }
        public bool CommentSelection(Editor kukaTextEditor)
        {
            if (kukaTextEditor == null)
            {
                throw new ArgumentNullException("kukaTextEditor");
            }
            bool result;
            using (kukaTextEditor.Document.RunUpdate())
            {
                var flag = false;
                if (kukaTextEditor.SelectionLength > 0)
                {
                    var lineByOffset = kukaTextEditor.Document.GetLineByOffset(kukaTextEditor.SelectionStart);
                    var lineByOffset2 = kukaTextEditor.Document.GetLineByOffset(kukaTextEditor.SelectionStart + kukaTextEditor.SelectionLength);
                    for (var i = lineByOffset.LineNumber - 1; i <= lineByOffset2.LineNumber - 1; i++)
                    {
                        var documentLine = kukaTextEditor.Document.Lines[i];
                        CommentLine(kukaTextEditor.Document, documentLine);
                        flag = true;
                    }
                    result = flag;
                }
                else
                {
                    if (CommentCarretLineIfNoSelection)
                    {
                        var lineByOffset3 = kukaTextEditor.Document.GetLineByOffset(kukaTextEditor.SelectionStart);
                        result = CommentLine(kukaTextEditor.Document, lineByOffset3);
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
        public bool UncommentSelection(Editor kukaTextEditor)
        {
            if (kukaTextEditor == null)
            {
                throw new ArgumentNullException("kukaTextEditor");
            }
            bool result;
            using (kukaTextEditor.Document.RunUpdate())
            {
                var flag = false;
                if (kukaTextEditor.SelectionLength > 0)
                {
                    var lineByOffset = kukaTextEditor.Document.GetLineByOffset(kukaTextEditor.SelectionStart);
                    var lineByOffset2 = kukaTextEditor.Document.GetLineByOffset(kukaTextEditor.SelectionStart + kukaTextEditor.SelectionLength);
                    for (var i = lineByOffset.LineNumber - 1; i <= lineByOffset2.LineNumber - 1; i++)
                    {
                        var documentLine = kukaTextEditor.Document.Lines[i];
                        flag |= UncommentLine(kukaTextEditor, documentLine);
                    }
                    result = flag;
                }
                else
                {
                    if (CommentCarretLineIfNoSelection)
                    {
                        var lineByOffset3 = kukaTextEditor.Document.GetLineByOffset(kukaTextEditor.SelectionStart);
                        result = UncommentLine(kukaTextEditor, lineByOffset3);
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
        public bool UncommentLine(Editor kukaTextEditor, DocumentLine documentLine)
        {
            if (kukaTextEditor == null)
            {
                throw new ArgumentNullException("kukaTextEditor");
            }
            if (documentLine == null)
            {
                throw new ArgumentNullException("documentLine");
            }
            var text = kukaTextEditor.Document.GetText(documentLine);
            var num = text.IndexOf(commentMarker, StringComparison.Ordinal);
            if (num <= -1)
            {
                return false;
            }
            var num2 = 0;
            var text2 = text;
            for (var i = 0; i < text2.Length; i++)
            {
                var c = text2[i];
                if (c == commentMarker[num2])
                {
                    num2++;
                    if (num2 >= commentMarker.Length)
                    {
                        kukaTextEditor.Document.Remove(documentLine.Offset + num, commentMarker.Length);
                        return true;
                    }
                }
                else
                {
                    if (!IsWhitespace(c))
                    {
                        break;
                    }
                }
            }
            return false;
        }
        private static bool IsWhitespace(char letter) => Whitespaces.Contains(letter);
        private bool CommentAtBeginOfLine(TextDocument document, DocumentLine documentLine)
        {
            var text = document.GetText(documentLine).TrimStart(Whitespaces.ToArray());
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            if (!AllowMultipleComments && text.StartsWith(CommentMarker))
            {
                return false;
            }
            document.Insert(documentLine.Offset, CommentMarker);
            return true;
        }
        private bool CommentAtBeginOfText(TextDocument document, DocumentLine documentLine)
        {
            var text = document.GetText(documentLine);
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            if (!AllowMultipleComments && text.TrimStart(Whitespaces.ToArray()).StartsWith(commentMarker))
            {
                return false;
            }
            var num = documentLine.Offset;
            var text2 = text;
            var i = 0;
            while (i < text2.Length)
            {
                var letter = text2[i];
                if (IsWhitespace(letter))
                {
                    num++;
                    i++;
                }
                else
                {
                    if (num >= documentLine.EndOffset)
                    {
                        return false;
                    }
                    break;
                }
            }
            document.Insert(num, CommentMarker);
            return true;
        }
    }
    public enum CommentMode
    {
        BeginOfLine,
        BeginOfText
    }
}
