using RobotEditor.Controls.TextEditor.Completion;
using RobotEditor.Interfaces;
using System;
using System.Collections.Generic;

namespace RobotEditor.Controls.TextEditor;

public interface IInsightWindow : ICompletionWindow
{
    IList<IInsightItem> Items { get; }
    IInsightItem SelectedItem { get; set; }
    event EventHandler<TextChangeEventArgs> DocumentChanged;
    event EventHandler SelectedItemChanged;
    event EventHandler CaretPositionChanged;
}