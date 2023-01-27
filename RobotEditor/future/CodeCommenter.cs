using ICSharpCode.AvalonEdit.Document;
using RobotEditor.Controls.TextEditor;
using System;
using System.Collections.Generic;

namespace RobotEditor.future
{
    public class CodeCommenter
    {
        private static readonly List<char> Whitespaces = new List<char>
        {
            ' ',
            '\t'
        };

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
        public string CommentMarker { get; set; } = ";";
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
        public bool CommentLine(AvalonEditor kukaTextEditor, DocumentLine documentLine) => CommentLine(kukaTextEditor.Document, documentLine);

        public bool CommentLine(TextDocument document, DocumentLine documentLine) => Mode == CommentMode.BeginOfLine ? CommentAtBeginOfLine(document, documentLine) : CommentAtBeginOfText(document, documentLine);
        public bool CommentLines(AvalonEditor kukaTextEditor, int startLine, int endLine)
        {
            if (endLine > kukaTextEditor.Document.LineCount)
            {
                throw new ArgumentOutOfRangeException("endLine", "End line must not be lager than the line count");
            }
            if (endLine < startLine)
            {
                throw new ArgumentException("The start line must be smaller than the end line");
            }
            bool flag = false;
            for (int i = startLine; i <= endLine; i++)
            {
                DocumentLine documentLine = kukaTextEditor.Document.Lines[i - 1];
                flag |= CommentLine(kukaTextEditor.Document, documentLine);
            }
            return flag;
        }
        public bool CommentLines(AvalonEditor kukaTextEditor, IEnumerable<DocumentLine> documentLines)
        {
            if (kukaTextEditor == null)
            {
                throw new ArgumentNullException("kukaTextEditor");
            }
            bool flag = false;
            foreach (DocumentLine current in documentLines)
            {
                flag |= CommentLine(kukaTextEditor.Document, current);
            }
            return flag;
        }
        public bool CommentSelection(AvalonEditor kukaTextEditor)
        {
            if (kukaTextEditor == null)
            {
                throw new ArgumentNullException("kukaTextEditor");
            }
            bool result;
            using (kukaTextEditor.Document.RunUpdate())
            {
                bool flag = false;
                if (kukaTextEditor.SelectionLength > 0)
                {
                    DocumentLine lineByOffset = kukaTextEditor.Document.GetLineByOffset(kukaTextEditor.SelectionStart);
                    DocumentLine lineByOffset2 = kukaTextEditor.Document.GetLineByOffset(kukaTextEditor.SelectionStart + kukaTextEditor.SelectionLength);
                    for (int i = lineByOffset.LineNumber - 1; i <= lineByOffset2.LineNumber - 1; i++)
                    {
                        DocumentLine documentLine = kukaTextEditor.Document.Lines[i];
                        _ = CommentLine(kukaTextEditor.Document, documentLine);
                        flag = true;
                    }
                    result = flag;
                }
                else
                {
                    if (CommentCarretLineIfNoSelection)
                    {
                        DocumentLine lineByOffset3 = kukaTextEditor.Document.GetLineByOffset(kukaTextEditor.SelectionStart);
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
        public bool UncommentSelection(AvalonEditor kukaTextEditor)
        {
            if (kukaTextEditor == null)
            {
                throw new ArgumentNullException("kukaTextEditor");
            }
            bool result;
            using (kukaTextEditor.Document.RunUpdate())
            {
                bool flag = false;
                if (kukaTextEditor.SelectionLength > 0)
                {
                    DocumentLine lineByOffset = kukaTextEditor.Document.GetLineByOffset(kukaTextEditor.SelectionStart);
                    DocumentLine lineByOffset2 = kukaTextEditor.Document.GetLineByOffset(kukaTextEditor.SelectionStart + kukaTextEditor.SelectionLength);
                    for (int i = lineByOffset.LineNumber - 1; i <= lineByOffset2.LineNumber - 1; i++)
                    {
                        DocumentLine documentLine = kukaTextEditor.Document.Lines[i];
                        flag |= UncommentLine(kukaTextEditor, documentLine);
                    }
                    result = flag;
                }
                else
                {
                    if (CommentCarretLineIfNoSelection)
                    {
                        DocumentLine lineByOffset3 = kukaTextEditor.Document.GetLineByOffset(kukaTextEditor.SelectionStart);
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
        public bool UncommentLine(AvalonEditor kukaTextEditor, DocumentLine documentLine)
        {
            if (kukaTextEditor == null)
            {
                throw new ArgumentNullException("kukaTextEditor");
            }
            if (documentLine == null)
            {
                throw new ArgumentNullException("documentLine");
            }
            string text = kukaTextEditor.Document.GetText(documentLine);
            int num = text.IndexOf(CommentMarker, StringComparison.Ordinal);
            if (num <= -1)
            {
                return false;
            }
            int num2 = 0;
            string text2 = text;
            for (int i = 0; i < text2.Length; i++)
            {
                char c = text2[i];
                if (c == CommentMarker[num2])
                {
                    num2++;
                    if (num2 >= CommentMarker.Length)
                    {
                        kukaTextEditor.Document.Remove(documentLine.Offset + num, CommentMarker.Length);
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
            string text = document.GetText(documentLine).TrimStart(Whitespaces.ToArray());
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
            string text = document.GetText(documentLine);
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }
            if (!AllowMultipleComments && text.TrimStart(Whitespaces.ToArray()).StartsWith(CommentMarker))
            {
                return false;
            }
            int num = documentLine.Offset;
            string text2 = text;
            int i = 0;
            while (i < text2.Length)
            {
                char letter = text2[i];
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
