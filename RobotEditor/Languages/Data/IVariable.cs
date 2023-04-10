using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace RobotEditor.Interfaces;

public interface IVariable
{
    bool IsSelected { get; set; }
    BitmapImage Icon { get; set; }
    string Name { get; set; }

    [Localizable(false)]
    string Type { get; set; }

    string Path { get; set; }
    string Value { get; set; }
    string Comment { get; set; }
    string Declaration { get; set; }
    string Description { get; set; }
    int Offset { get; set; }
}