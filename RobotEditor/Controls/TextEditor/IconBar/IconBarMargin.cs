using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using RobotEditor.Controls.TextEditor.Bookmarks;
using RobotEditor.Interfaces;

namespace RobotEditor.Controls.TextEditor.IconBar
{
    public sealed class IconBarMargin : AbstractMargin, IDisposable
    {
        private readonly IBookmarkMargin _manager;
        private IBookmark _dragDropBookmark;
        private double _dragDropCurrentPoint;
        private double _dragDropStartPoint;
        private bool _dragStarted;

        public IconBarMargin(IBookmarkMargin manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            _manager = manager;
        }

        public void Dispose()
        {
            TextView = null;
            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Called when the <see cref="P:ICSharpCode.AvalonEdit.Editing.AbstractMargin.TextView" /> is changing.
        /// </summary>
        protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
        {
            if (oldTextView != null)
            {
                oldTextView.VisualLinesChanged -= OnRedrawRequested;
                _manager.RedrawRequested -= OnRedrawRequested;
            }
            base.OnTextViewChanged(oldTextView, newTextView);
            if (newTextView != null)
            {
                newTextView.VisualLinesChanged += OnRedrawRequested;
                _manager.RedrawRequested += OnRedrawRequested;
            }

            //            Console.WriteLine(Properties.Resources.IconBarMargin_OnTextViewChanged_Fix_On_Text_View_Changed_in_IconBarMargin);
            InvalidateVisual();
        }

        [DebuggerStepThrough]
        private void OnRedrawRequested(object sender, EventArgs e)
        {
            if (TextView != null && TextView.VisualLinesValid)
            {
                InvalidateVisual();
            }
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) => new PointHitTestResult(this, hitTestParameters.HitPoint);

        [DebuggerStepThrough]
        protected override Size MeasureOverride(Size availableSize) => new Size(18.0, 0.0);

        [DebuggerStepThrough]
        protected override void OnRender(DrawingContext drawingContext)
        {
            var renderSize = RenderSize;
            drawingContext.DrawRectangle(SystemColors.ControlBrush, null,
                new Rect(0, 0, renderSize.Width, renderSize.Height));
            drawingContext.DrawLine(new Pen(SystemColors.ControlDarkBrush, 1),
                new Point(renderSize.Width - 0.5, 0),
                new Point(renderSize.Width - 0.5, renderSize.Height));

            var textView = TextView;
            if (textView == null || !textView.VisualLinesValid) return;
            // create a dictionary line number => first bookmark
            var bookmarkDict = new Dictionary<int, IBookmark>();
            foreach (var bm in _manager.Bookmarks)
            {
                var line = bm.LineNumber;
                IBookmark existingBookmark;
                if (!bookmarkDict.TryGetValue(line, out existingBookmark) || bm.ZOrder > existingBookmark.ZOrder)
                    bookmarkDict[line] = bm;
            }
            var pixelSize = PixelSnapHelpers.GetPixelSize(this);
            Rect rect;
            foreach (var line in textView.VisualLines)
            {
                var lineNumber = line.FirstDocumentLine.LineNumber;
                IBookmark bm;

                if (!bookmarkDict.TryGetValue(lineNumber, out bm)) continue;
                var lineMiddle = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.TextMiddle) -
                                    textView.VerticalOffset;
                rect = new Rect(0, PixelSnapHelpers.Round(lineMiddle - 8, pixelSize.Height), 16, 16);
                if (_dragDropBookmark == bm && _dragStarted)
                    drawingContext.PushOpacity(0.5);
                drawingContext.DrawImage((bm.Image ?? BookmarkBase.defaultBookmarkImage).Bitmap, rect);
                if (_dragDropBookmark == bm && _dragStarted)
                    drawingContext.Pop();
            }
            if (_dragDropBookmark == null || !_dragStarted) return;
            rect = new Rect(0, PixelSnapHelpers.Round(_dragDropCurrentPoint - 8, pixelSize.Height), 16, 16);
            drawingContext.DrawImage((_dragDropBookmark.Image ?? BookmarkBase.defaultBookmarkImage).ImageSource, rect);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            CancelDragDrop();
            base.OnMouseDown(e);
            var lineFromMousePosition = GetLineFromMousePosition(e);
            if (!e.Handled && lineFromMousePosition > 0)
            {
                var bookmarkFromLine = GetBookmarkFromLine(lineFromMousePosition);
                if (bookmarkFromLine != null)
                {
                    bookmarkFromLine.MouseDown(e);
                    if (!e.Handled)
                    {
                        if (e.ChangedButton == MouseButton.Left && bookmarkFromLine.CanDragDrop && CaptureMouse())
                        {
                            StartDragDrop(bookmarkFromLine, e);
                            e.Handled = true;
                        }
                    }
                }
            }
            if (e.ChangedButton == MouseButton.Left)
            {
                e.Handled = true;
            }
        }

        private IBookmark GetBookmarkFromLine(int line)
        {
            IBookmark[] result = { null };
            foreach (
                var bm in
                    _manager.Bookmarks.Where(bm => bm.LineNumber == line)
                        .Where(bm => result[0] == null || bm.ZOrder > result[0].ZOrder))
            {
                result[0] = bm;
            }
            return result[0];
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            CancelDragDrop();
            base.OnLostMouseCapture(e);
        }

        private void StartDragDrop(IBookmark bm, MouseEventArgs e)
        {
            _dragDropBookmark = bm;
            _dragDropStartPoint = _dragDropCurrentPoint = e.GetPosition(this).Y;
            if (TextView != null)
            {
                if (TextView.Services.GetService(typeof(TextArea)) is TextArea textArea)
                {
                    textArea.PreviewKeyDown += TextAreaPreviewKeyDown;
                }
            }
        }

        private void CancelDragDrop()
        {
            if (_dragDropBookmark != null)
            {
                _dragDropBookmark = null;
                _dragStarted = false;
                if (TextView != null)
                {
                    if (TextView.Services.GetService(typeof(TextArea)) is TextArea textArea)
                    {
                        textArea.PreviewKeyDown -= TextAreaPreviewKeyDown;
                    }
                }
                ReleaseMouseCapture();
                InvalidateVisual();
            }
        }

        private void TextAreaPreviewKeyDown(object sender, KeyEventArgs e)
        {
            CancelDragDrop();
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
            }
        }

        private int GetLineFromMousePosition(MouseEventArgs e)
        {
            var textView = TextView;
            int result;
            if (textView == null)
            {
                result = 0;
            }
            else
            {
                var visualLineFromVisualTop =
                    textView.GetVisualLineFromVisualTop(e.GetPosition(textView).Y + textView.ScrollOffset.Y);
                result = visualLineFromVisualTop == null ? 0 : visualLineFromVisualTop.FirstDocumentLine.LineNumber;
            }
            return result;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_dragDropBookmark != null)
            {
                _dragDropCurrentPoint = e.GetPosition(this).Y;
                if (Math.Abs(_dragDropCurrentPoint - _dragDropStartPoint) > SystemParameters.MinimumVerticalDragDistance)
                {
                    _dragStarted = true;
                }
                InvalidateVisual();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            var lineFromMousePosition = GetLineFromMousePosition(e);
            if (!e.Handled && _dragDropBookmark != null)
            {
                if (_dragStarted)
                {
                    if (lineFromMousePosition != 0)
                    {
                        _dragDropBookmark.Drop(lineFromMousePosition);
                    }
                    e.Handled = true;
                }
                CancelDragDrop();
            }
            if (!e.Handled && lineFromMousePosition != 0)
            {
                var bookmarkFromLine = GetBookmarkFromLine(lineFromMousePosition);
                if (bookmarkFromLine != null)
                {
                    bookmarkFromLine.MouseUp(e);
                    if (e.Handled)
                    {
                        return;
                    }
                }
                if (e.ChangedButton == MouseButton.Left && TextView != null)
                {
                    if (TextView.Services.GetService(typeof(ITextEditor)) is ITextEditor textEditor)
                    {
                    }
                }
            }
        }

        // no bookmark on the line: create a new breakpoint


        //		ITextEditor textEditor = TextView.Services.GetService(typeof(ITextEditor)) as ITextEditor;
        //		if (textEditor != null) {
        //			DebuggerService.ToggleBreakpointAt(textEditor, line, typeof(BreakpointBookmark));
        //			return;
        //		}
        //		
        //		// create breakpoint for the other posible active contents
        //		var viewContent = WorkbenchSingleton.Workbench.ActiveContent as AbstractViewContentWithoutFile;
        //		if (viewContent != null) {
        //			textEditor = viewContent.Services.GetService(typeof(ITextEditor)) as ITextEditor;
        //			if (textEditor != null) {
        //				DebuggerService.ToggleBreakpointAt(textEditor, line, typeof(DecompiledBreakpointBookmark));
        //				return;
    }
}