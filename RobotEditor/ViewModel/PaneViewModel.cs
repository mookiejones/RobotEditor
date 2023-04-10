using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace RobotEditor.ViewModel;

public partial class PaneViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string _contentId;
    [ObservableProperty]
    private bool _isActive;
    [ObservableProperty]
    private bool _isSelected;
    [ObservableProperty]
    private string _title;



    public ImageSource IconSource { get; set; }


}