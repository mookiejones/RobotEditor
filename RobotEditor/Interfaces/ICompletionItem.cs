using RobotEditor.Abstract;

namespace RobotEditor.Interfaces
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