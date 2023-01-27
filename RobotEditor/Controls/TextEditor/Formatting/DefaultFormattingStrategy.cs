using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using RobotEditor.Interfaces;

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
            var document = editor.Document;
            var lineNumber = line.LineNumber;
            if (lineNumber <= 1)
            {
                return;
            }
            document.GetLine(lineNumber - 1);
            throw new NotImplementedException();
        }

        public virtual void IndentLines(ITextEditor editor, int begin, int end)
        {
            using (editor.Document.OpenUndoGroup())
            {
                for (var i = begin; i <= end; i++)
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
                var location = editor.Document.OffsetToPosition(editor.SelectionStart);
                var location2 = editor.Document.OffsetToPosition(editor.SelectionStart + editor.SelectionLength);
                var num = location2.Column == 1 && location2.Line > location.Line
                    ? location2.Line - 1
                    : location2.Line;
                var list = new List<IEditorDocumentLine>();
                var flag = true;
                for (var i = location.Line; i <= num; i++)
                {
                    list.Add(editor.Document.GetLine(i));
                    if (!list[i - location.Line].Text.Trim().StartsWith(comment, StringComparison.Ordinal))
                    {
                        flag = false;
                    }
                }
                foreach (var current in list)
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
                var offset = editor.SelectionStart;
                var offset2 = editor.SelectionStart + editor.SelectionLength;
                if (editor.SelectionLength == 0)
                {
                    var lineForOffset = editor.Document.GetLineForOffset(editor.SelectionStart);
                    offset = lineForOffset.Offset;
                    offset2 = lineForOffset.Offset + lineForOffset.Length;
                }
                var blockCommentRegion = FindSelectedCommentRegion(editor, blockStart, blockEnd);
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
            var document = editor.Document;
            BlockCommentRegion result;
            if (document.TextLength == 0)
            {
                result = null;
            }
            else
            {
                var selectedText = editor.SelectedText;
                var num = selectedText.IndexOf(commentStart, StringComparison.Ordinal);
                if (num >= 0)
                {
                    num += editor.SelectionStart;
                }
                var num2 = num >= 0
                    ? selectedText.IndexOf(commentEnd, num + commentStart.Length - editor.SelectionStart,
                        StringComparison.Ordinal)
                    : selectedText.IndexOf(commentEnd, StringComparison.Ordinal);
                if (num2 >= 0)
                {
                    num2 += editor.SelectionStart;
                }
                if (num == -1)
                {
                    var num3 = editor.SelectionStart + editor.SelectionLength + commentStart.Length - 1;
                    if (num3 > document.TextLength)
                    {
                        num3 = document.TextLength;
                    }
                    var text = document.GetText(0, num3);
                    num = text.LastIndexOf(commentStart, StringComparison.Ordinal);
                    if (num >= 0)
                    {
                        var num4 = text.IndexOf(commentEnd, num, editor.SelectionStart - num, StringComparison.Ordinal);
                        if (num4 > num)
                        {
                            num = -1;
                        }
                    }
                }
                if (num2 == -1)
                {
                    var num3 = editor.SelectionStart + 1 - commentEnd.Length;
                    if (num3 < 0)
                    {
                        num3 = editor.SelectionStart;
                    }
                    var text = document.GetText(num3, document.TextLength - num3);
                    num2 = text.IndexOf(commentEnd, StringComparison.Ordinal);
                    if (num2 >= 0)
                    {
                        num2 += num3;
                    }
                }
                if (num != -1 && num2 != -1)
                {
                    result = new BlockCommentRegion(commentStart, commentEnd, num, num2);
                }
                else
                {
                    result = null;
                }
            }
            return result;
        }
    }
}