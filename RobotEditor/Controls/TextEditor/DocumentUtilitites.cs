using ICSharpCode.AvalonEdit.Document;
using RobotEditor.Utilities;
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
                    return -1;
                
                char charAt = document.GetCharAt(num);
                if (!charAt.IsWordPart() && !allowedChars.Contains(charAt))
                    return num;

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
                if (!charAt.IsWhitespaceOrNewline())
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
                if (!charAt.IsWhitespaceOrNewline())
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
                if (!charAt.IsWordPart())
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
            public static int FindPrevWordStart(this TextDocument document, int offset, IList<char> allowedChars)
        {
            for (int num = offset - 1; num != -1; num--)
            {
                char charAt = document.GetCharAt(num);
                if (!charAt.IsWordPart() && !allowedChars.Contains(charAt))
                {
                    return num + 1;
                }
            }
            return 0;
        }
       
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
    }


}