using System;
using System.Collections.Generic;

namespace RobotEditor.Controls.TextEditor.Bookmarks
{
    public interface IBookmarkMargin
    {
        IList<IBookmark> Bookmarks { get; }
        event EventHandler RedrawRequested;
        void Redraw();
    }
}