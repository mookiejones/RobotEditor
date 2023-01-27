using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using RobotEditor.Interfaces;

namespace RobotEditor.Classes
{
    public sealed class BookmarkImage : IImage
    {
        private readonly IImage _baseimage = null;
        private readonly BitmapImage _bitmap;

        public BookmarkImage(BitmapImage bitmap)
        {
            _bitmap = bitmap;
        }

        public ImageSource ImageSource => _baseimage.ImageSource;

        public BitmapImage Bitmap => _bitmap;

        public Icon Icon => _baseimage.Icon;
    }
}