using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace RobotEditor.ViewModel;

public sealed class StatusBarViewModel : ObservableRecipient
{



    private static StatusBarViewModel _instance;
    private bool _isCapsPressed;
    private bool _isInsPressed;
    private bool _isNumPressed;
    private bool _isScrollPressed;
    private RelayCommand<object> _keyPressedCommand;

    public StatusBarViewModel()
    {
        GetInitialKeyState();
    }




    public RelayCommand<object> KeyPressedCommand => _keyPressedCommand ??= new RelayCommand<object>(param => ManageKeys(param, null));

    public static StatusBarViewModel Instance => _instance ??= new StatusBarViewModel();

    public bool IsScrollPressed { get => _isScrollPressed; set => SetProperty(ref _isScrollPressed, value); }

    public bool IsNumPressed { get => _isNumPressed; set => SetProperty(ref _isNumPressed, value); }

    public bool IsInsPressed { get => _isInsPressed; set => SetProperty(ref _isInsPressed, value); }

    public bool IsCapsPressed { get => _isCapsPressed; set => SetProperty(ref _isCapsPressed, value); }

    public void ManageKeys(object sender, KeyEventArgs e)
    {
        if (e != null)
        {
            Key key = e.Key;
            if (key != Key.Capital)
            {
                if (key != Key.Insert)
                {
                    switch (key)
                    {
                        case Key.NumLock:
                            IsNumPressed = e.IsToggled;
                            break;
                        case Key.Scroll:
                            IsScrollPressed = e.IsToggled;
                            break;
                    }
                }
                else
                {
                    IsInsPressed = e.IsToggled;
                }
            }
            else
            {
                IsCapsPressed = e.IsToggled;
            }
        }
    }

    private void GetInitialKeyState()
    {
        IsCapsPressed = NativeMethods.GetKeyState(20) != 0;
        IsInsPressed = NativeMethods.GetKeyState(45) != 0;
        IsNumPressed = NativeMethods.GetKeyState(144) != 0;
        IsScrollPressed = NativeMethods.GetKeyState(145) != 0;
    }

    private enum VKeyStates
    {
        CapsKey = 20,
        NumKey = 144,
        ScrollKey,
        InsKey = 45
    }
}