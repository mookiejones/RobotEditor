using CommunityToolkit.Mvvm.Messaging;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Messages;
using RobotEditor.Views;
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RobotEditor.Controls;

public sealed class ExplorerClass : System.Windows.Forms.TreeView, IComparable
{
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
        DirectoryInfo directoryInfo = (DirectoryInfo)obj;
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
            string fullPath = SelectedNode.FullPath;
            SelectedFile = File.Exists(fullPath) ? fullPath : string.Empty;
            SelectedDirectory = Directory.Exists(fullPath) ? fullPath : string.Empty;
        }
        base.OnAfterSelect(e);
    }

    public void ShowTree()
    {
        base.Nodes.Clear();
        try
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drives)
            {
                switch (driveInfo.DriveType)
                {
                    case DriveType.Unknown:
                        AddNode(driveInfo.Name, 7, 7);
                        break;
                    case DriveType.Removable:
                        {
                            string name = driveInfo.Name;
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
            ErrorMessage msg = new("ExplorerClass", ex, MessageType.Error);
            _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
        }
    }

    private void AddNode(string name, int unselected, int selected)
    {
        TreeNode treeNode = new(name, unselected, selected);
        _ = Nodes.Add(treeNode);
        treeNode.Tag = name;
        _ = treeNode.Nodes.Add(string.Empty);
    }

    public void ShowTree(string path, bool bArchiveRoot, string sRobName, bool bSelect)
    {
        int num = 5;
        string text = path;
        if (bArchiveRoot)
        {
            num = 11;
            text = sRobName;
        }
        TreeNode treeNode = new(text, num, num);
        if (bArchiveRoot)
        {
            treeNode.Tag = path;
            _ = Nodes.Add(treeNode);
        }
        else
        {
            _ = Nodes.Add(treeNode);
        }
        FillTreeNode(treeNode, string.Empty);
        if (bSelect)
        {
            SelectedNode = treeNode;
        }
    }

    public void ShowTree(DriveType driveType)
    {
        var items = DriveInfo.GetDrives()
                .Where(driveInfo => driveInfo.DriveType == driveType);

        foreach (var item in items)
            ShowTree(item.Name, false, "", false);
    }

    [Localizable(false)]
    public void FillTreeNode(TreeNode node, string root)
    {
        if (root == null)
        {
            throw new ArgumentNullException(nameof(root));
        }
        try
        {
            Cursor = Cursors.WaitCursor;
            string text = node.FullPath;
            if (string.CompareOrdinal(text, "\\") == 0)
            {
                text = node.ToString();
            }
            else
            {
                if (string.CompareOrdinal(text.Substring(1, 1), ":") != 0)
                {
                    root = node.Text;
                    text = root + text[text.IndexOf("\\", StringComparison.Ordinal)..];
                }
            }
            DirectoryInfo directoryInfo = new(text);
            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            Comparer comparer = new(CultureInfo.InvariantCulture);
            Array.Sort(directories, comparer);
            foreach (TreeNode current in
                from d in directories
                select new TreeNode(d.Name, 0, 1)
                {
                    Tag = node.Tag.ToString()
                })
            {
                _ = node.Nodes.Add(current);
                _ = current.Nodes.Add("");
            }
            string[] files = Directory.GetFiles(text, FileExplorerControl.Instance.Filter);
            Array.Sort(files);
            string[] array = files;
            string[] array2 = array;
            foreach (string path in array2)
            {
                TreeNode treeNode = new(Path.GetFileName(path))
                {
                    Tag = node.Tag.ToString()
                };
                string extension = Path.GetExtension(path);
                if (extension != null)
                {
                    string text2 = extension.ToLower();
                    string text3 = text2;
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
                _ = node.Nodes.Add(treeNode);
            }
            Cursor = Cursors.Default;
        }
        catch (Exception ex)
        {
            ErrorMessage msg = new("ExplorerClass.FillTreeNode", ex, MessageType.Error);
            _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
            Cursor = Cursors.Default;
        }
    }

    protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
    {
        TreeNode node = e.Node;
        base.BeginUpdate();
        node.Nodes.Clear();
        string root = e.Node.Tag.ToString();
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