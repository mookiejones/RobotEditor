using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace RobotEditor.Controls.TextEditor
{
    internal sealed class CaretHighlightAdorner : Adorner
    {
        private readonly RectangleGeometry _geometry;
        private readonly Pen _pen;

        public CaretHighlightAdorner(TextArea textArea)
            : base(textArea.TextView)
        {
            Rect rect = textArea.Caret.CalculateCaretRectangle();
            rect.Offset(-textArea.TextView.ScrollOffset);
            Rect toValue = rect;
            double num = Math.Max(rect.Width, rect.Height) * 0.25;
            toValue.Inflate(num, num);
            _pen = new Pen(TextBlock.GetForeground(textArea.TextView).Clone(), 1.0);
            _geometry = new RectangleGeometry(rect, 2.0, 2.0);
            _geometry.BeginAnimation(RectangleGeometry.RectProperty,
                new RectAnimation(rect, toValue, new Duration(TimeSpan.FromMilliseconds(300.0)))
                {
                    AutoReverse = true
                });
            _pen.Brush.BeginAnimation(Brush.OpacityProperty,
                new DoubleAnimation(1.0, 0.0, new Duration(TimeSpan.FromMilliseconds(200.0)))
                {
                    BeginTime = TimeSpan.FromMilliseconds(450.0)
                });
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(null, _pen, _geometry);
        }
    }
}