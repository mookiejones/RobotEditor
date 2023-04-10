using CommunityToolkit.Mvvm.ComponentModel;
using RobotEditor.Enums;
using RobotEditor.ViewModel;

namespace RobotEditor.Tools.Notes;

public sealed partial class NotesViewModel : ToolViewModel
{
    [ObservableProperty]
    private string text = string.Empty;
   

    public NotesViewModel()
        : base("Notes")
    {
        ContentId = "NotesTool";
        DefaultPane = DefaultToolPane.Bottom;
    }
}