
using CommunityToolkit.Mvvm.ComponentModel;

namespace RobotEditor.ViewModel;

public sealed partial class FileOptionsViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _showFullName = true;
 
}