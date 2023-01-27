using ICSharpCode.AvalonEdit.Document;
using RobotEditor.Interfaces;
using System;
using System.Windows.Input;
using ITextAnchor = RobotEditor.Interfaces.ITextAnchor;

namespace RobotEditor.Controls.TextEditor.Bookmarks
{
    public class BookmarkBase : IBookmark
    {
        public static readonly IImage defaultBookmarkImage = null;
        private IEditor _document;
        private TextLocation _location;

        public BookmarkBase(TextLocation location)
        {
            Location = location;
        }

        public IEditor Document
        {
            get => _document;
            set
            {
                if (_document != value)
                {
                    if (Anchor != null)
                    {
                        throw new NotImplementedException();
                        //                        _location = Anchor.Location;
                        Anchor = null;
                    }
                    _document = value;
                    CreateAnchor();
                    OnDocumentChanged(EventArgs.Empty);
                }
            }
        }

        public ITextAnchor Anchor { get; private set; }

        public TextLocation Location
        {
            get => throw new NotImplementedException();//                return (Anchor != null) ? Anchor.Location : _location;
            set
            {
                _location = value;
                CreateAnchor();
            }
        }

        public int ColumnNumber => Anchor != null ? Anchor.Column : _location.Column;

        public virtual bool CanToggle => true;

        public static IImage DefaultBookmarkImage => defaultBookmarkImage;

        public int LineNumber => Anchor != null ? Anchor.Line : _location.Line;

        public virtual int ZOrder => 0;

        public virtual IImage Image => defaultBookmarkImage;

        public virtual bool CanDragDrop => false;

        public virtual void MouseDown(MouseButtonEventArgs e)
        {
        }

        public virtual void MouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && CanToggle)
            {
                RemoveMark();
                e.Handled = true;
            }
        }

        public virtual void Drop(int lineNumber)
        {
        }

        public event EventHandler DocumentChanged;

        private void CreateAnchor()
        {
            if (_document != null)
            {
                int num = Math.Max(1, Math.Min(_location.Line, _document.TotalNumberOfLines));
                int length = _document.GetLine(num).Length;
                int offset = _document.PositionToOffset(num, Math.Max(1, Math.Min(_location.Column, length + 1)));
                Anchor = _document.CreateAnchor(offset);
                Anchor.MovementType = AnchorMovementType.AfterInsertion;
                Anchor.Deleted += AnchorDeleted;
            }
            else
            {
                Anchor = null;
            }
        }

        private void AnchorDeleted(object sender, EventArgs e)
        {
            //            _location = Location.Empty;
            Anchor = null;
            RemoveMark();
        }

        protected virtual void RemoveMark()
        {
            if (_document != null)
            {
                if (_document.GetService(typeof(IBookmarkMargin)) is IBookmarkMargin bookmarkMargin)
                {
                    _ = bookmarkMargin.Bookmarks.Remove(this);
                }
            }
        }

        protected virtual void OnDocumentChanged(EventArgs e)
        {
            DocumentChanged?.Invoke(this, e);
        }

        protected virtual void Redraw()
        {
            if (_document != null)
            {
                if (_document.GetService(typeof(IBookmarkMargin)) is IBookmarkMargin bookmarkMargin)
                {
                    bookmarkMargin.Redraw();
                }
            }
        }
    }
}