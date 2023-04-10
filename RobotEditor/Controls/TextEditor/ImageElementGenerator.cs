using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RobotEditor.Controls.TextEditor;

public class ImageElementGenerator : VisualLineElementGenerator
{
    private static readonly Regex ImageRegex = new("<img src=\"([\\.\\/\\w\\d]+)\"/?>",
        RegexOptions.IgnoreCase);

    private readonly string _basePath;

    public ImageElementGenerator(string basePath)
    {
        _basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
    }

    private Match FindMatch(int startOffset)
    {
        int endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
        ICSharpCode.AvalonEdit.Document.TextDocument document = CurrentContext.Document;
        string text = document.GetText(startOffset, endOffset - startOffset);
        return ImageRegex.Match(text);
    }

    public override int GetFirstInterestedOffset(int startOffset)
    {
        Match match = FindMatch(startOffset);
        return match.Success ? startOffset + match.Index : -1;
    }

    public override VisualLineElement ConstructElement(int offset)
    {
        Match match = FindMatch(offset);
        VisualLineElement result;
        if (match.Success && match.Index == 0)
        {
            BitmapImage bitmapImage = LoadBitmap(match.Groups[1].Value);
            if (bitmapImage != null)
            {
                Image element = new()
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
            string text = Path.Combine(_basePath, fileName);
            if (File.Exists(text))
            {
                BitmapImage bitmapImage = new(new Uri(text));
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