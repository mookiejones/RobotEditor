using System;
using System.Collections.Generic;

namespace RobotEditor.Interfaces
{
    public interface IBookmarkMargin
    {
        IList<IBookmark> Bookmarks { get; }
        event EventHandler RedrawRequested;
        void Redraw();
    }
}