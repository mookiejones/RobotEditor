using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace RobotEditor.Utilities
{
    internal static class ImageHelper
    {
        public static BitmapImage LoadBitmap(Bitmap img)
        {
            var bitmapImage = new BitmapImage();
            using (var memoryStream = new MemoryStream())
            {
                img.Save(memoryStream, ImageFormat.Jpeg);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        public static ImageSource GetIcon(string fileName)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(fileName);
            bitmapImage.EndInit();
            return bitmapImage;
        }

        public static BitmapImage LoadBitmap(string fileName)
        {
            BitmapImage result;

#if DEBUG
            var file = new System.IO.FileInfo(fileName);
            if (!file.Exists)
            {
                Console.WriteLine(file);
            }
#endif
            try
            {
                if (File.Exists(fileName))
                {
                    var fileInfo = new System.IO.FileInfo(fileName);
                    var bitmapImage = new BitmapImage(new Uri(fileInfo.FullName));
                    bitmapImage.Freeze();
                    result = bitmapImage;
                    return result;
                }
            }
            catch (ArgumentException)
            {
            }
            catch (IOException)
            {
            }
            result = null;
            return result;
        }
    }
}
