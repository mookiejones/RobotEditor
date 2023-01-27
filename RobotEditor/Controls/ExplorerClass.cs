using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.Messaging;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Messages;
using RobotEditor.Views;

namespace RobotEditor.Controls
{
    public sealed class ExplorerClass : TreeView, IComparable
    {
// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
        private const int FOLDER = 0;
        private const int FOLDEROPEN = 1;
        private const int REMOVABLE = 5;
        private const int CDDRIVE = 3;
        private const int FIXEDDRIVE = 2;
        private const int GENERICFILE = 6;
        private const int NETWORK = 7;

        public ExplorerClass()
        {
            base.HideSelection = false;
        }

        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Local
        public string SelectedFile { get; set; }
        public string SelectedDirectory { get; set; }

        public int CompareTo(object obj)
        {
            var directoryInfo = (DirectoryInfo) obj;
            return string.CompareOrdinal(Name, directoryInfo.Name);
        }

        public event FileSelectedEventHandler OnFileSelected;

        public void AddRobotNode(string sFile, string sName)
        {
            SelectedNode = Nodes.Add(sFile, sName, 11, 11);
            SelectedNode.ToolTipText = sFile;
        }

        private void RaiseFileSelected(object sender, FileSelectedEventArgs e) => OnFileSelected?.Invoke(sender, e);

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (File.Exists(SelectedNode.FullPath))
            {
                RaiseFileSelected(this, new FileSelectedEventArgs
                {
                    Filename = SelectedNode.FullPath
                });
            }
            base.OnMouseDoubleClick(e);
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            if (SelectedNode != null)
            {
                var fullPath = SelectedNode.FullPath;
                SelectedFile = (File.Exists(fullPath) ? fullPath : string.Empty);
                SelectedDirectory = (Directory.Exists(fullPath) ? fullPath : string.Empty);
            }
            base.OnAfterSelect(e);
        }

        public void ShowTree()
        {
            base.Nodes.Clear();
            try
            {
                var drives = DriveInfo.GetDrives();
                foreach (var driveInfo in drives)
                {
                    switch (driveInfo.DriveType)
                    {
                        case DriveType.Unknown:
                            AddNode(driveInfo.Name, 7, 7);
                            break;
                        case DriveType.Removable:
                        {
                            var name = driveInfo.Name;
                            if (name == null)
                            {
                                goto IL_9E;
                            }
                            if (name != "A:\\" && name != "B:\\")
                            {
                                goto IL_9E;
                            }
                            AddNode(driveInfo.Name, 5, 5);
                            break;
                            IL_9E:
                            AddNode(driveInfo.Name, 5, 5);
                            break;
                        }
                        case DriveType.Fixed:
                            AddNode(driveInfo.Name, 2, 2);
                            break;
                        case DriveType.Network:
                            AddNode(driveInfo.Name, 7, 7);
                            break;
                        case DriveType.CDRom:
                            AddNode(driveInfo.Name, 3, 3);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = new ErrorMessage("ExplorerClass", ex, MessageType.Error);
                WeakReferenceMessenger.Default.Send<IMessage>(msg);
            }
        }

        private void AddNode(string name, int unselected, int selected)
        {
            var treeNode = new TreeNode(name, unselected, selected);
            Nodes.Add(treeNode);
            treeNode.Tag = name;
            treeNode.Nodes.Add(string.Empty);
        }

        public void ShowTree(string path, bool bArchiveRoot, string sRobName, bool bSelect)
        {
            var num = 5;
            var text = path;
            if (bArchiveRoot)
            {
                num = 11;
                text = sRobName;
            }
            var treeNode = new TreeNode(text, num, num);
            if (bArchiveRoot)
            {
                treeNode.Tag = path;
                Nodes.Add(treeNode);
            }
            else
            {
                Nodes.Add(treeNode);
            }
            FillTreeNode(treeNode, string.Empty);
            if (bSelect)
            {
                SelectedNode = treeNode;
            }
        }

        public void ShowTree(DriveType driveType)
        {
            var drives = DriveInfo.GetDrives();
            foreach (var current in
                from driveInfo in drives
                where driveInfo.DriveType == driveType
                select driveInfo)
            {
                ShowTree(current.Name, false, "", false);
            }
        }

        [Localizable(false)]
        public void FillTreeNode(TreeNode node, string root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            try
            {
                Cursor = Cursors.WaitCursor;
                var text = node.FullPath;
                if (string.CompareOrdinal(text, "\\") == 0)
                {
                    text = node.ToString();
                }
                else
                {
                    if (string.CompareOrdinal(text.Substring(1, 1), ":") != 0)
                    {
                        root = node.Text;
                        text = root + text.Substring(text.IndexOf("\\", StringComparison.Ordinal));
                    }
                }
                var directoryInfo = new DirectoryInfo(text);
                var directories = directoryInfo.GetDirectories();
                var comparer = new Comparer(CultureInfo.InvariantCulture);
                Array.Sort(directories, comparer);
                foreach (var current in
                    from d in directories
                    select new TreeNode(d.Name, 0, 1)
                    {
                        Tag = node.Tag.ToString()
                    })
                {
                    node.Nodes.Add(current);
                    current.Nodes.Add("");
                }
                var files = Directory.GetFiles(text, FileExplorerControl.Instance.Filter);
                Array.Sort(files);
                var array = files;
                var array2 = array;
                foreach (var path in array2)
                {
                    var treeNode = new TreeNode(Path.GetFileName(path))
                    {
                        Tag = node.Tag.ToString()
                    };
                    var extension = Path.GetExtension(path);
                    if (extension != null)
                    {
                        var text2 = extension.ToLower();
                        var text3 = text2;
                        if (text3 == null)
                        {
                            goto IL_260;
                        }
                        if (!(text3 == ".src"))
                        {
                            if (!(text3 == ".dat"))
                            {
                                if (!(text3 == ".sub"))
                                {
                                    if (!(text3 == ".zip"))
                                    {
                                        goto IL_260;
                                    }
                                    treeNode.SelectedImageIndex = 6;
                                    treeNode.ImageIndex = 6;
                                }
                                else
                                {
                                    treeNode.SelectedImageIndex = 6;
                                    treeNode.ImageIndex = 6;
                                }
                            }
                            else
                            {
                                treeNode.SelectedImageIndex = 6;
                                treeNode.ImageIndex = 6;
                            }
                        }
                        else
                        {
                            treeNode.SelectedImageIndex = 6;
                            treeNode.ImageIndex = 6;
                        }
                        goto IL_275;
                        IL_260:
                        treeNode.SelectedImageIndex = 6;
                        treeNode.ImageIndex = 6;
                    }
                    IL_275:
                    node.Nodes.Add(treeNode);
                }
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                var msg = new ErrorMessage("ExplorerClass.FillTreeNode", ex, MessageType.Error);
                WeakReferenceMessenger.Default.Send<IMessage>(msg);
                Cursor = Cursors.Default;
            }
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            var node = e.Node;
            base.BeginUpdate();
            node.Nodes.Clear();
            var root = e.Node.Tag.ToString();
            FillTreeNode(node, root);
            base.EndUpdate();
            base.OnBeforeExpand(e);
        }

        public class FileSelectedEventArgs : EventArgs
        {
            public string Filename { get; set; }
        }
    }

    public delegate void FileSelectedEventHandler(object sender, ExplorerClass.FileSelectedEventArgs e);
}