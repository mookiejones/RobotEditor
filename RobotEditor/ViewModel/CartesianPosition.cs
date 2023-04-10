
using CommunityToolkit.Mvvm.ComponentModel;

namespace RobotEditor.ViewModel;

public sealed partial class CartesianPosition : ObservableObject
{
    [ObservableProperty]
    private string _header = string.Empty;

    [ObservableProperty]
    private double _x;
    [ObservableProperty]
    private double _y;
    [ObservableProperty]
    private double _z;

}