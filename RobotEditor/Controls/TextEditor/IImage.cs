using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RobotEditor.Controls.TextEditor
{
    public interface IImage
    {
        ImageSource ImageSource { get; }
        BitmapImage Bitmap { get; }
        Icon Icon { get; }
    }
}