using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RobotEditor.Controls.TextEditor.Bookmarks;

public sealed class BookmarkImage : IImage
{
    private readonly IImage _baseimage = null;

    public BookmarkImage(BitmapImage bitmap)
    {
        Bitmap = bitmap;
    }

    public ImageSource ImageSource => _baseimage.ImageSource;

    public BitmapImage Bitmap { get; }

    public Icon Icon => _baseimage.Icon;
}