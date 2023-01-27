using RobotEditor.Controls.TextEditor.Snippets.CompletionData;

namespace RobotEditor.Controls.TextEditor.Completion
{
    public interface ICompletionItem
    {
        string Text { get; }
        string Description { get; }
        IImage Image { get; }
        double Priority { get; }
        void Complete(CompletionContext context);
    }
}