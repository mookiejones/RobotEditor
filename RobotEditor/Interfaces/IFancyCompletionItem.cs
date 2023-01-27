namespace RobotEditor.Interfaces
{
    public interface IFancyCompletionItem : ICompletionItem
    {
        object Content { get; }
    }
}