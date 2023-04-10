using RobotEditor.Enums;

namespace RobotEditor.ViewModel;

public sealed class ShiftViewModel : ToolViewModel
{
    private static ShiftViewModel _instance;

    private CartesianPosition _diffvalues = new()
    {
        Header = "Difference"
    };

    private CartesianPosition _newvalues = new()
    {
        Header = "New Values"
    };

    private CartesianPosition _oldvalues = new()
    {
        Header = "Old Values"
    };

    public ShiftViewModel()
        : base("Shift Program")
    {
        DefaultPane = DefaultToolPane.Right;
    }

    public CartesianPosition OldValues { get => _oldvalues; set => SetProperty(ref _oldvalues, value); }

    public CartesianPosition NewValues { get => _newvalues; set => SetProperty(ref _newvalues, value); }

    public CartesianPosition DiffValues { get => _diffvalues; set => SetProperty(ref _diffvalues, value); }

    public static ShiftViewModel Instance
    {
        get => _instance ?? new ShiftViewModel();
        set => _instance = value;
    }
}