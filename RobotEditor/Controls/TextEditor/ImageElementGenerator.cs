using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.Rendering;

namespace RobotEditor.Classes
{
    public class ImageElementGenerator : VisualLineElementGenerator
    {
        private static readonly Regex ImageRegex = new Regex("<img src=\"([\\.\\/\\w\\d]+)\"/?>",
            RegexOptions.IgnoreCase);

        private readonly string _basePath;

        public ImageElementGenerator(string basePath)
        {
            if (basePath == null)
            {
                throw new ArgumentNullException("basePath");
            }
            _basePath = basePath;
        }

        private Match FindMatch(int startOffset)
        {
            var endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            var document = CurrentContext.Document;
            var text = document.GetText(startOffset, endOffset - startOffset);
            return ImageRegex.Match(text);
        }

        public override int GetFirstInterestedOffset(int startOffset)
        {
            var match = FindMatch(startOffset);
            return match.Success ? (startOffset + match.Index) : -1;
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            var match = FindMatch(offset);
            VisualLineElement result;
            if (match.Success && match.Index == 0)
            {
                var bitmapImage = LoadBitmap(match.Groups[1].Value);
                if (bitmapImage != null)
                {
                    var element = new Image
                    {
                        Source = bitmapImage,
                        Width = bitmapImage.PixelWidth,
                        Height = bitmapImage.PixelHeight
                    };
                    result = new InlineObjectElement(match.Length, element);
                    return result;
                }
            }
            result = null;
            return result;
        }

        private BitmapImage LoadBitmap(string fileName)
        {
            BitmapImage result;
            try
            {
                var text = Path.Combine(_basePath, fileName);
                if (File.Exists(text))
                {
                    var bitmapImage = new BitmapImage(new Uri(text));
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