using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;

namespace RobotEditor.Controls.TextEditor
{
    public static class DocumentUtilitites
    {
        public static int FindNextWordEnd(this TextDocument document, int offset) => document.FindNextWordEnd(offset, new List<char>());

        public static int FindNextWordEnd(this TextDocument document, int offset, IList<char> allowedChars)
        {
            for (int num = offset; num != -1; num++)
            {
                if (num >= document.TextLength)
                {
                    return -1;
                }
                char charAt = document.GetCharAt(num);
                if (!IsWordPart(charAt) && !allowedChars.Contains(charAt))
                {
                    return num;
                }
            }
            return -1;
        }
        public static int FindNextWordStart(this TextDocument document, int offset)
        {
            for (int num = offset; num != -1; num++)
            {
                if (num >= document.TextLength)
                {
                    return 0;
                }
                char charAt = document.GetCharAt(num);
                if (!IsWhitespaceOrNewline(charAt))
                {
                    return num;
                }
            }
            return 0;
        }
        public static int FindNextWordStartRelativeTo(this TextDocument document, int offset)
        {
            for (int num = offset; num != -1; num++)
            {
                char charAt = document.GetCharAt(num);
                if (!IsWhitespaceOrNewline(charAt))
                {
                    return num - offset;
                }
            }
            return 0;
        }
        public static int FindPrevWordStart(this TextDocument document, int offset)
        {
            for (int num = offset - 1; num != -1; num--)
            {
                char charAt = document.GetCharAt(num);
                if (!IsWordPart(charAt))
                {
                    return num + 1;
                }
            }
            return 0;
        }
        public static ISegment GetLineWithoutIndent(this TextDocument document, int lineNumber)
        {
            DocumentLine lineByNumber = document.GetLineByNumber(lineNumber);
            ISegment whitespaceAfter = TextUtilities.GetWhitespaceAfter(document, lineByNumber.Offset);
            return whitespaceAfter.Length == 0
                ? lineByNumber
                : (ISegment)new TextSegment
                {
                    StartOffset = lineByNumber.Offset + whitespaceAfter.Length,
                    EndOffset = lineByNumber.EndOffset,
                    Length = lineByNumber.Length - whitespaceAfter.Length
                };
        }
        public static string GetWordBeforeCaret(this AvalonEditor editor)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }
            int offset = editor.TextArea.Caret.Offset;
            int num = editor.Document.FindPrevWordStart(offset);
            return num < 0 ? string.Empty : editor.Document.GetText(num, offset - num);
        }
        public static string GetWordBeforeCaret(this AvalonEditor editor, char[] allowedChars)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }
            int offset = editor.TextArea.Caret.Offset;
            int num = FindPrevWordStart(editor.Document, offset, allowedChars);
            return num < 0 ? string.Empty : editor.Document.GetText(num, offset - num);
        }
        public static string GetStringBeforeCaret(this AvalonEditor editor)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }
            int line = editor.TextArea.Caret.Line;
            if (line < 1)
            {
                return string.Empty;
            }
            int offset = editor.TextArea.Caret.Offset;
            if (line > editor.Document.LineCount)
            {
                return string.Empty;
            }
            DocumentLine lineByNumber = editor.Document.GetLineByNumber(line);
            int length = offset - lineByNumber.Offset;
            return editor.Document.GetText(lineByNumber.Offset, length);
        }
        public static string GetWordBeforeOffset(this AvalonEditor editor, int offset, char[] allowedChars)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }
            int num = FindPrevWordStart(editor.Document, offset, allowedChars);
            return num < 0 ? string.Empty : editor.Document.GetText(num, offset - num);
        }
        public static string GetTokenBeforeOffset(this AvalonEditor editor, int offset)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }
            int num = -1;
            for (int i = offset - 1; i > -1; i--)
            {
                char charAt = editor.Document.GetCharAt(i);
                if (charAt == ' ' || charAt == '\n' || charAt == '\r' || charAt == '\t')
                {
                    num = i + 1;
                    break;
                }
            }
            return num < 0 ? string.Empty : editor.Document.GetText(num, offset - num);
        }
        public static string GetWordUnderCaret(this AvalonEditor editor, char[] allowedChars)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }
            int offset = editor.TextArea.Caret.Offset;
            int num = FindPrevWordStart(editor.Document, offset, allowedChars);
            int num2 = editor.Document.FindNextWordEnd(offset, allowedChars);
            return num < 0 || num2 == 0 || num2 < num ? string.Empty : editor.Document.GetText(num, num2 - num);
        }
        public static string GetFirstWordInLine(this AvalonEditor editor, int lineNumber) => editor == null ? throw new ArgumentNullException("editor") : editor.Document.GetFirstWordInLine(lineNumber);
        public static string GetFirstWordInLine(this TextDocument document, int lineNumber)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            int offset = document.GetOffset(lineNumber, 0);
            int num = document.FindNextWordStart(offset);
            if (num < 0)
            {
                return string.Empty;
            }
            int num2 = document.FindNextWordEnd(num);
            return num2 < 0 ? string.Empty : document.GetText(num, num2 - num);
        }
        public static string GetWordUnderCaret(this AvalonEditor editor)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }
            int offset = editor.TextArea.Caret.Offset;
            int num = editor.Document.FindPrevWordStart(offset);
            int num2 = editor.Document.FindNextWordEnd(offset);
            return num < 0 || num2 == 0 || num2 < num ? string.Empty : editor.Document.GetText(num, num2 - num);
        }
        public static string GetWordUnderOffset(this AvalonEditor editor, int offset)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }
            int num = editor.Document.FindPrevWordStart(offset);
            int num2 = editor.Document.FindNextWordEnd(offset);
            return num < 0 || num2 == 0 || num2 < num ? string.Empty : editor.Document.GetText(num, num2 - num);
        }
        public static string GetWordUnderOffset(this AvalonEditor editor, int offset, char[] allowedChars)
        {
            if (editor == null)
            {
                throw new ArgumentNullException("editor");
            }
            int num = FindPrevWordStart(editor.Document, offset, allowedChars);
            int num2 = editor.Document.FindNextWordEnd(offset, allowedChars);
            return num < 0 || num2 == 0 || num2 < num ? string.Empty : editor.Document.GetText(num, num2 - num);
        }
        private static int FindPrevWordStart(TextDocument document, int offset, IList<char> allowedChars)
        {
            for (int num = offset - 1; num != -1; num--)
            {
                char charAt = document.GetCharAt(num);
                if (!IsWordPart(charAt) && !allowedChars.Contains(charAt))
                {
                    return num + 1;
                }
            }
            return 0;
        }
        public static bool IsWhitespaceOrNewline(char ch) => ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';

        private static bool IsWordPart(char ch) => char.IsLetterOrDigit(ch) || ch == '_';
    }


}
