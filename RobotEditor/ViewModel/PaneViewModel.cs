using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace RobotEditor.ViewModel;

public partial class PaneViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string contentId;

    [ObservableProperty]
    private bool isActive;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private string title;



    public ImageSource? IconSource { get; set; }


}