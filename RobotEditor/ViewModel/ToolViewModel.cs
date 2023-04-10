using CommunityToolkit.Mvvm.ComponentModel;
using RobotEditor.Enums;

namespace RobotEditor.ViewModel;

public partial class ToolViewModel : PaneViewModel
{
    public DefaultToolPane DefaultPane = DefaultToolPane.None;

    protected ToolViewModel(string name)
    {
        Name = name;
        Title = name;
    }

    public int Height { get; set; }
    public int Width { get; set; }
    public string Name { get; private set; }

    [ObservableProperty]
    private bool _isVisible = true;
}