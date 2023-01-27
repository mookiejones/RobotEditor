using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using RobotEditor.Interfaces;

namespace RobotEditor.Classes
{
    public sealed class IconBarManager : IBookmarkMargin
    {
        private readonly ObservableCollection<IBookmark> _bookmarks = new ObservableCollection<IBookmark>();

        public IconBarManager()
        {
            _bookmarks.CollectionChanged += BookmarksCollectionChanged;
        }

        public event EventHandler RedrawRequested;

        public IList<IBookmark> Bookmarks => _bookmarks;

        public void Redraw() => RedrawRequested?.Invoke(this, EventArgs.Empty);

        private void BookmarksCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => Redraw();

        public void AddBookMark(UIElement item)
        {
        }
    }
}