using System;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace RobotEditor.Controls.TextEditor.Snippets
{
    public class SnippetCompletionData : TextEditor.CompletionData
    {
        private readonly SnippetInfo snippetInfo;
        public override object Content => snippetInfo.Header.Text;
        public override object Description => new SnippetToolTip(snippetInfo);
        public override ImageSource Image => null;//TODO Add this//                return Resources.snippet.ToBitmapSource();
        public override double Priority => 100.0;
        public override string Text => snippetInfo.Header.Text;
        public SnippetCompletionData(SnippetInfo snippetInfo)
        {
            this.snippetInfo = snippetInfo;
        }
        public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            if (snippetInfo.Header.Types.Contains(SnippetType.Expansion) && textArea.Selection.IsEmpty)
            {
                ReplaceIfNeeded(textArea, snippetInfo);
                snippetInfo.Snippet.Insert(textArea);
                return;
            }
            if (snippetInfo.Header.Types.Contains(SnippetType.SurroundsWith) && !textArea.Selection.IsEmpty)
            {
                snippetInfo.Snippet.Insert(textArea);
            }
        }
        private static bool IsWhitespace(char ch) => ch == '\t' || ch == ' ' || ch == '\n';
        private bool ReplaceIfNeeded(TextArea area, SnippetInfo snippInfo)
        {
            var i = area.Caret.Offset;
            var shortcuts = snippInfo.Header.Shortcuts;
            var num = -1;
            var document = area.Document;
            if (i <= 0)
            {
                return false;
            }
            while (i > 0)
            {
                if (i >= document.TextLength)
                {
                    i--;
                }
                var charAt = document.GetCharAt(i);
                if (IsWhitespace(charAt))
                {
                    num = i + 1;
                    break;
                }
                i--;
                num = i;
            }
            if (num < area.Caret.Offset)
            {
                num = Math.Max(num, 0);
                var length = area.Caret.Offset - num;
                var text = document.GetText(num, length);
                if (shortcuts.Any((string shortcut) => shortcut.Contains(text)))
                {
                    document.Replace(num, length, string.Empty);
                    return true;
                }
            }
            return false;
        }
    }
}