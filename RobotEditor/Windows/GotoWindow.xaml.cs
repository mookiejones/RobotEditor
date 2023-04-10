using System.Windows;
using System.Windows.Controls;

namespace RobotEditor.Windows;

/// <summary>
///     Interaction logic for GotoWindow.xaml
/// </summary>
public sealed partial class GotoWindow
{
    public GotoWindow()
    {
        InitializeComponent();
    }

    private void Close(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            string text = button.Content.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                if (text != "_OK")
                {
                    if (text == "_Cancel")
                    {
                        DialogResult = false;
                    }
                }
                else
                {
                    DialogResult = true;
                }
            }
        }
    }
}