using ICSharpCode.AvalonEdit.Document;
using RobotEditor.Interfaces;
using System;
using System.Collections.Generic;

namespace RobotEditor.Controls.TextEditor.Formatting
{
    public class DefaultFormattingStrategy : IFormattingStrategy
    {
        internal static readonly DefaultFormattingStrategy DefaultInstance = new DefaultFormattingStrategy();

        public virtual void FormatLine(ITextEditor editor, char charTyped)
        {
        }

        public virtual void IndentLine(ITextEditor editor, IDocumentLine line)
        {
            IEditor document = editor.Document;
            int lineNumber = line.LineNumber;
            if (lineNumber <= 1)
            {
                return;
            }
            _ = document.GetLine(lineNumber - 1);
            throw new NotImplementedException();
        }

        public virtual void IndentLines(ITextEditor editor, int begin, int end)
        {
            using (editor.Document.OpenUndoGroup())
            {
                for (int i = begin; i <= end; i++)
                {
                    IndentLine(editor, editor.Document.GetLine(i));
                }
            }
        }

        public virtual void SurroundSelectionWithComment(ITextEditor editor)
        {
        }

        protected void SurroundSelectionWithSingleLineComment(ITextEditor editor, string comment)
        {
            using (editor.Document.OpenUndoGroup())
            {
                Structs.Location location = editor.Document.OffsetToPosition(editor.SelectionStart);
                Structs.Location location2 = editor.Document.OffsetToPosition(editor.SelectionStart + editor.SelectionLength);
                int num = location2.Column == 1 && location2.Line > location.Line
                    ? location2.Line - 1
                    : location2.Line;
                List<IEditorDocumentLine> list = new List<IEditorDocumentLine>();
                bool flag = true;
                for (int i = location.Line; i <= num; i++)
                {
                    list.Add(editor.Document.GetLine(i));
                    if (!list[i - location.Line].Text.Trim().StartsWith(comment, StringComparison.Ordinal))
                    {
                        flag = false;
                    }
                }
                foreach (IEditorDocumentLine current in list)
                {
                    if (flag)
                    {
                        editor.Document.Remove(
                            current.Offset + current.Text.IndexOf(comment, StringComparison.Ordinal), comment.Length);
                    }
                    else
                    {
                        editor.Document.Insert(current.Offset, comment, AnchorMovementType.BeforeInsertion);
                    }
                }
            }
        }

        protected void SurroundSelectionWithBlockComment(ITextEditor editor, string blockStart, string blockEnd)
        {
            using (editor.Document.OpenUndoGroup())
            {
                int offset = editor.SelectionStart;
                int offset2 = editor.SelectionStart + editor.SelectionLength;
                if (editor.SelectionLength == 0)
                {
                    IEditorDocumentLine lineForOffset = editor.Document.GetLineForOffset(editor.SelectionStart);
                    offset = lineForOffset.Offset;
                    offset2 = lineForOffset.Offset + lineForOffset.Length;
                }
                BlockCommentRegion blockCommentRegion = FindSelectedCommentRegion(editor, blockStart, blockEnd);
                if (blockCommentRegion != null)
                {
                    editor.Document.Remove(blockCommentRegion.EndOffset, blockCommentRegion.CommentEnd.Length);
                    editor.Document.Remove(blockCommentRegion.StartOffset, blockCommentRegion.CommentStart.Length);
                }
                else
                {
                    editor.Document.Insert(offset2, blockEnd);
                    editor.Document.Insert(offset, blockStart);
                }
            }
        }

        public static BlockCommentRegion FindSelectedCommentRegion(ITextEditor editor, string commentStart,
            string commentEnd)
        {
            IEditor document = editor.Document;
            BlockCommentRegion result;
            if (document.TextLength == 0)
            {
                result = null;
            }
            else
            {
                string selectedText = editor.SelectedText;
                int num = selectedText.IndexOf(commentStart, StringComparison.Ordinal);
                if (num >= 0)
                {
                    num += editor.SelectionStart;
                }
                int num2 = num >= 0
                    ? selectedText.IndexOf(commentEnd, num + commentStart.Length - editor.SelectionStart,
                        StringComparison.Ordinal)
                    : selectedText.IndexOf(commentEnd, StringComparison.Ordinal);
                if (num2 >= 0)
                {
                    num2 += editor.SelectionStart;
                }
                if (num == -1)
                {
                    int num3 = editor.SelectionStart + editor.SelectionLength + commentStart.Length - 1;
                    if (num3 > document.TextLength)
                    {
                        num3 = document.TextLength;
                    }
                    string text = document.GetText(0, num3);
                    num = text.LastIndexOf(commentStart, StringComparison.Ordinal);
                    if (num >= 0)
                    {
                        int num4 = text.IndexOf(commentEnd, num, editor.SelectionStart - num, StringComparison.Ordinal);
                        if (num4 > num)
                        {
                            num = -1;
                        }
                    }
                }
                if (num2 == -1)
                {
                    int num3 = editor.SelectionStart + 1 - commentEnd.Length;
                    if (num3 < 0)
                    {
                        num3 = editor.SelectionStart;
                    }
                    string text = document.GetText(num3, document.TextLength - num3);
                    num2 = text.IndexOf(commentEnd, StringComparison.Ordinal);
                    if (num2 >= 0)
                    {
                        num2 += num3;
                    }
                }
                result = num != -1 && num2 != -1 ? new BlockCommentRegion(commentStart, commentEnd, num, num2) : null;
            }
            return result;
        }
    }
}