using CommunityToolkit.Mvvm.ComponentModel;

namespace RobotEditor.ViewModel;

public sealed class KFDDialogViewModel : ObservableRecipient
{
    private int _answer;
    private bool _b1Visible = true;
    private bool _b2Visible = true;
    private bool _b3Visible = true;
    private bool _b4Visible = true;
    private bool _b5Visible = true;
    private bool _b6Visible = true;
    private bool _b7Visible = true;
    private string _button1Text = "button1";
    private string _button2Text = "button2";
    private string _button3Text = "button3";
    private string _button4Text = "button4";
    private string _button5Text = "button5";
    private string _button6Text = "button6";
    private string _button7Text = "button7";
    private int _width = 592;

    public KFDDialogViewModel()
    {
        Button7Visible = !string.IsNullOrEmpty(Button7Text);
        if (!Button7Visible)
        {
            Width = -81;
        }
        Button6Visible = !string.IsNullOrEmpty(Button6Text);
        if (!Button6Visible)
        {
            Width = -81;
        }
        Button5Visible = !string.IsNullOrEmpty(Button5Text);
        if (!Button5Visible)
        {
            Width = -81;
        }
        Button4Visible = !string.IsNullOrEmpty(Button4Text);
        if (!Button4Visible)
        {
            Width = -81;
        }
        Button3Visible = !string.IsNullOrEmpty(Button3Text);
        if (!Button3Visible)
        {
            Width = -81;
        }
        Button2Visible = !string.IsNullOrEmpty(Button2Text);
        if (!Button2Visible)
        {
            Width = -81;
        }
        Button1Visible = !string.IsNullOrEmpty(Button1Text);
        if (!Button1Visible)
        {
            Width = -81;
        }
    }

    public string Button1Text { get => _button1Text; set => SetProperty(ref _button1Text, value); }

    public string Button2Text { get => _button2Text; set => SetProperty(ref _button2Text, value); }

    public string Button3Text { get => _button3Text; set => SetProperty(ref _button3Text, value); }

    public string Button4Text { get => _button4Text; set => SetProperty(ref _button4Text, value); }

    public string Button5Text { get => _button5Text; set => SetProperty(ref _button5Text, value); }

    public string Button6Text { get => _button6Text; set => SetProperty(ref _button6Text, value); }

    public string Button7Text { get => _button7Text; set => SetProperty(ref _button7Text, value); }

    public bool Button1Visible { get => _b1Visible; set => SetProperty(ref _b1Visible, value); }

    public bool Button2Visible { get => _b2Visible; set => SetProperty(ref _b2Visible, value); }

    public bool Button3Visible { get => _b3Visible; set => SetProperty(ref _b3Visible, value); }

    public bool Button4Visible { get => _b4Visible; set => SetProperty(ref _b4Visible, value); }

    public bool Button5Visible { get => _b5Visible; set => SetProperty(ref _b5Visible, value); }

    public bool Button6Visible { get => _b6Visible; set => SetProperty(ref _b6Visible, value); }

    public bool Button7Visible { get => _b7Visible; set => SetProperty(ref _b7Visible, value); }

    public int Width { get => _width; set => SetProperty(ref _width, value); }

    public int Answer { get => _answer; set => SetProperty(ref _answer, value); }
}