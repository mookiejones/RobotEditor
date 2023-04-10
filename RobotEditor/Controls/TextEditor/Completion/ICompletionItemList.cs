using RobotEditor.Controls.TextEditor.Snippets.CompletionData;
using RobotEditor.Enums;
using System.Collections.Generic;

namespace RobotEditor.Controls.TextEditor.Completion;

public interface ICompletionItemList
{
    IEnumerable<ICompletionItem> Items { get; }
    ICompletionItem SuggestedItem { get; }
    int PreselectionLength { get; }
    bool ContainsAllAvailableItems { get; }
    CompletionItemListKeyResult ProcessInput(char key);
    void Complete(CompletionContext context, ICompletionItem item);
}