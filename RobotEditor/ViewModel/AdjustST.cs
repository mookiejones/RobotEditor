

using CommunityToolkit.Mvvm.ComponentModel;

namespace RobotEditor.ViewModel;

internal sealed partial class AdjustST : ObservableObject
{
    [ObservableProperty]
    private ToolItems _toolItems = new();
}