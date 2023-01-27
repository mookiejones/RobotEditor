namespace RobotEditor.Controls.TextEditor.Completion
{
    public interface IFancyCompletionItem : ICompletionItem
    {
        object Content { get; }
    }
}