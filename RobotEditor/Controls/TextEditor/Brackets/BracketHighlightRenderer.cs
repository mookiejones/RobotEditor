using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Windows.Media;

namespace RobotEditor.Controls.TextEditor.Brackets;

public sealed class BracketHighlightRenderer : IBackgroundRenderer
{
    private static readonly Color DefaultBackground = Color.FromArgb(100, 0, 0, 255);
    private readonly TextView _textView;
    private Brush _backgroundBrush;
    private Pen _borderPen;
    private BracketSearchResult _result;

    public BracketHighlightRenderer(TextView textView)
    {
        _textView = textView ?? throw new ArgumentNullException(nameof(textView));
        _textView.BackgroundRenderers.Add(this);
    }

    public KnownLayer Layer => KnownLayer.Selection;

    public void Draw(TextView textview, DrawingContext drawingContext)
    {
        if (_result != null)
        {
            BackgroundGeometryBuilder backgroundGeometryBuilder = new()
            {
                CornerRadius = 1.0,
                AlignToWholePixels = true
            };
            backgroundGeometryBuilder.AddSegment(textview, new TextSegment
            {
                StartOffset = _result.OpeningBracketOffset,
                Length = _result.OpeningBracketLength
            });
            backgroundGeometryBuilder.CloseFigure();
            backgroundGeometryBuilder.AddSegment(textview, new TextSegment
            {
                StartOffset = _result.ClosingBracketOffset,
                Length = _result.ClosingBracketLength
            });
            Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
            if (_borderPen == null)
            {
                UpdateColors(DefaultBackground, DefaultBackground);
            }
            if (geometry != null)
            {
                drawingContext.DrawGeometry(_backgroundBrush, _borderPen, geometry);
            }
        }
    }

    public void SetHighlight(BracketSearchResult result)
    {
        if (_result != result)
        {
            _result = result;
            _textView.InvalidateLayer(Layer);
        }
    }

    private void UpdateColors(Color background, Color foreground)
    {
        _borderPen = new Pen(new SolidColorBrush(foreground), 1.0);
        _borderPen.Freeze();
        _backgroundBrush = new SolidColorBrush(background);
        _backgroundBrush.Freeze();
    }
}