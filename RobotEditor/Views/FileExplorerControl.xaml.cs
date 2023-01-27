using System;
using System.ComponentModel;
using System.Windows.Forms;
using RobotEditor.Controls;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;
using UserControl = System.Windows.Controls.UserControl;

namespace RobotEditor.Views
{
    /// <summary>
    ///     Interaction logic for FileExplorerControl.xaml
    /// </summary>
    public sealed partial class FileExplorerControl : UserControl
    {
        // ReSharper disable UnassignedField.Compiler
        // ReSharper disable UnusedField.Compiler
        private readonly ExplorerClass _explorer = new ExplorerClass();
        private ContextMenuStrip _ctxFileExplorer;
        private string _filter = "*.*";
        private ImageList _imgList;
        private ToolStripMenuItem _mnuCopy;
        private ToolStripMenuItem _mnuCut;
        private ToolStripMenuItem _mnuDelete;
        private ToolStripMenuItem _mnuPaste;
        private ToolStripMenuItem _mnuRefresh;

        public FileExplorerControl()
        {
            InitializeComponent();
            Instance = this;
            _explorer.ShowTree();
        }

        public static FileExplorerControl Instance { get; set; }

        [Localizable(false)]
        public string Filter
        {
            get => string.IsNullOrEmpty(_filter) ? "*.*" : _filter;
            set => _filter = value;
        }

        // ReSharper restore UnassignedField.Compiler
        // ReSharper restore UnusedField.Compiler
        public event FileSelectedEventHandler FileSelected
        {
            add => _explorer.OnFileSelected += value;
            remove => _explorer.OnFileSelected -= value;
        }

        public event TreeNodeMouseClickEventHandler OnMouseClick;
        public new event KeyEventHandler OnKeyUp;
        public event TreeViewEventHandler OnAfterSelect;

        private void RaiseAfterSelect(object sender, TreeViewEventArgs e) => OnAfterSelect?.Invoke(sender, e);

        private void RaiseKeyUp(object sender, KeyEventArgs e) => OnKeyUp?.Invoke(sender, e);

        private void RaiseMouseClick(object sender, TreeNodeMouseClickEventArgs e) => OnMouseClick?.Invoke(sender, e);

        private void CopyFile(object sender, EventArgs e)
        {
        }

        private void CutFile(object sender, EventArgs e)
        {
        }

        private void PasteFile(object sender, EventArgs e)
        {
        }

        private void DeleteFile(object sender, EventArgs e)
        {
        }

        private void Refresh(object sender, EventArgs e)
        {
            if (_explorer.SelectedNode != null)
            {
                _explorer.SelectedNode.Nodes.Clear();
                _explorer.FillTreeNode(_explorer.SelectedNode, _explorer.SelectedNode.Tag.ToString());
            }
        }

        private void ContextOpening(object sender, CancelEventArgs e)
        {
            var enabled = !string.IsNullOrEmpty(_explorer.SelectedFile) ||
                           !string.IsNullOrEmpty(_explorer.SelectedDirectory);
            _mnuCopy.Enabled = enabled;
            _mnuCut.Enabled = enabled;
            _mnuDelete.Enabled = enabled;
            _mnuPaste.Enabled = enabled;
        }
    }
}