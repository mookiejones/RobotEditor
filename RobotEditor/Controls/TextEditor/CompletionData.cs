﻿using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using RobotEditor.Controls.TextEditor.Snippets.CompletionData;

namespace RobotEditor.Controls.TextEditor
{
    public abstract class CompletionData : ICompletionData
    {
        protected double priority;
        public virtual object Content => Text;
        public virtual object Description => "Description";
        public virtual ImageSource Image => null;
        public virtual double Priority => priority;
        public abstract string Text
        {
            get;
        }
        protected string UsageName
        {
            get;
            set;
        }
        public virtual void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            if (!(textArea.GetService(typeof(Editor)) is Editor textEditor))
            {
                return;
            }
            var text = (!(textEditor is Editor kukaTextEditor)) ? textEditor.GetWordBeforeCaret() : kukaTextEditor.GetWordBeforeCaret(kukaTextEditor.GetWordParts());
            if (Text.StartsWith(text, StringComparison.InvariantCultureIgnoreCase) || Text.ToLowerInvariant().Contains(text.ToLowerInvariant()))
            {
                textEditor.Document.Replace(textEditor.CaretOffset - text.Length, text.Length, Text);
            }
            else
            {
                textEditor.Document.Insert(textEditor.CaretOffset, Text);
            }
            if (UsageName != null)
            {
                CodeCompletionDataUsageCache.IncrementUsage(UsageName);
            }
        }
    }

}
