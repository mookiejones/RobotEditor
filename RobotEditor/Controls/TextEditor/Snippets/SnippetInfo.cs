using ICSharpCode.AvalonEdit.Snippets;

namespace RobotEditor.Controls.TextEditor.Snippets
{
    public class SnippetInfo
    {
        public string Version
        {
            get;
            set;
        }
        public string Path
        {
            get;
            private set;
        }
        public string Filename
        {
            get
            {
                if (string.IsNullOrEmpty(Path))
                {
                    return string.Empty;
                }
                return System.IO.Path.GetFileName(Path);
            }
        }
        public SnippetHeader Header
        {
            get;
            set;
        }
        public Snippet Snippet
        {
            get;
            set;
        }
        public SnippetInfo()
        {
        }
        internal SnippetInfo(string path)
        {
            Path = path;
        }
    }
}