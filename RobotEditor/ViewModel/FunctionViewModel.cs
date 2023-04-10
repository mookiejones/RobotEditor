using RobotEditor.Enums;

namespace RobotEditor.ViewModel;

public sealed class FunctionViewModel : ToolViewModel
{
    private const string ToolContentId = "FunctionTool";

    public FunctionViewModel()
        : base("Functions")
    {
        ContentId = ToolContentId;
        DefaultPane = DefaultToolPane.Right;
    }
}