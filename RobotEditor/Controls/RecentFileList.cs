using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Messages;
using RobotEditor.ViewModel;

namespace RobotEditor.Controls
{
    [Localizable(false)]
    public class RecentFileList : Separator
    {
        public delegate string GetMenuItemTextDelegate(int index, string filepath);

        private static RecentFileList _instance;
        private List<RecentFile> _recentFiles;
        private Separator _separator;

        public RecentFileList()
        {
            Persister = new RegistryPersister();
            MaxNumberOfFiles = 9;
            MaxPathLength = 50;
            MenuItemFormatOneToNine = "_{0}:  {2}";
            MenuItemFormatTenPlus = "{0}:  {2}";
            Loaded += (s, e) => HookFileMenu();
        }

        public static RecentFileList Instance
        {
            get
            {
                RecentFileList arg_15_0;
                if ((arg_15_0 = _instance) == null)
                {
                    arg_15_0 = (_instance = new RecentFileList());
                }
                return arg_15_0;
            }
        }

        private IPersist Persister { get; set; }
        private int MaxNumberOfFiles { get; set; }
        private int MaxPathLength { get; set; }
        private MenuItem FileMenu { get; set; }
        private string MenuItemFormatOneToNine { get; set; }
        public string MenuItemFormatTenPlus { get; set; }
        public GetMenuItemTextDelegate GetMenuItemTextHandler { get; set; }

        public List<string> RecentFiles => Persister.RecentFiles(MaxNumberOfFiles);

        public void UseRegistryPersister() => Persister = new RegistryPersister();

        public void UseRegistryPersister(string key) => Persister = new RegistryPersister(key);

        public void UseXmlPersister() => Persister = new XmlPersister();

        public void UseXmlPersister(string filepath) => Persister = new XmlPersister(filepath);

        public void UseXmlPersister(Stream stream) => Persister = new XmlPersister(stream);

        private void HookFileMenu()
        {
            if (!(base.Parent is MenuItem menuItem))
            {
                throw new ApplicationException("Parent must be a MenuItem");
            }
            if (!Equals(FileMenu, menuItem))
            {
                if (FileMenu != null)
                {
                    FileMenu.SubmenuOpened -= FileMenuSubmenuOpened;
                }
                FileMenu = menuItem;
                FileMenu.SubmenuOpened += FileMenuSubmenuOpened;
            }
        }

        public void RemoveFile(string filepath) => Persister.RemoveFile(filepath, MaxNumberOfFiles);

        public void InsertFile(string filepath) => Persister.InsertFile(filepath, MaxNumberOfFiles);

        private void FileMenuSubmenuOpened(object sender, RoutedEventArgs e) => SetMenuItems();

        private void SetMenuItems()
        {
            RemoveMenuItems();
            LoadRecentFiles();
            InsertMenuItems();
        }

        private void RemoveMenuItems()
        {
            if (_separator != null)
            {
                FileMenu.Items.Remove(_separator);
            }
            if (_recentFiles != null)
            {
                foreach (var current in
                    from r in _recentFiles
                    where r.MenuItem != null
                    select r)
                {
                    FileMenu.Items.Remove(current.MenuItem);
                }
            }
            _separator = null;
            _recentFiles = null;
        }

        private void InsertMenuItems()
        {
            if (_recentFiles != null)
            {
                if (_recentFiles.Count != 0)
                {
                    var num = FileMenu.Items.IndexOf(this);
                    foreach (var current in _recentFiles)
                    {
                        var menuItemText = GetMenuItemText(current.Number + 1, current.Filepath, current.DisplayPath);
                        current.MenuItem = new MenuItem
                        {
                            Header = menuItemText
                        };
                        current.MenuItem.Click += MenuItemClick;
                        FileMenu.Items.Insert(++num, current.MenuItem);
                    }
                    _separator = new Separator();
                    FileMenu.Items.Insert(++num, _separator);
                }
            }
        }

        private string GetMenuItemText(int index, string filepath, string displaypath)
        {
            var getMenuItemTextHandler = GetMenuItemTextHandler;
            string result;
            if (getMenuItemTextHandler != null)
            {
                result = getMenuItemTextHandler(index, filepath);
            }
            else
            {
                var format = (index < 10) ? MenuItemFormatOneToNine : MenuItemFormatTenPlus;
                var arg = ShortenPathname(displaypath, MaxPathLength);
                result = string.Format(format, index, filepath, arg);
            }
            return result;
        }

        private static string ShortenPathname(string pathname, int maxLength)
        {
            string result;
            if (pathname.Length <= maxLength)
            {
                result = pathname;
            }
            else
            {
                var text = Path.GetPathRoot(pathname);
                if (text.Length > 3)
                {
                    text += Path.DirectorySeparatorChar;
                }
                var array = pathname.Substring(text.Length).Split(new[]
                {
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                });
                var num = array.GetLength(0) - 1;
                if (array.GetLength(0) == 1)
                {
                    if (array[0].Length > 5)
                    {
                        if (text.Length + 6 >= maxLength)
                        {
                            result = text + array[0].Substring(0, 3) + "...";
                        }
                        else
                        {
                            result = pathname.Substring(0, maxLength - 3) + "...";
                        }
                    }
                    else
                    {
                        result = pathname;
                    }
                }
                else
                {
                    if (text.Length + 4 + array[num].Length > maxLength)
                    {
                        text += "...\\";
                        var num2 = array[num].Length;
                        if (num2 < 6)
                        {
                            result = text + array[num];
                        }
                        else
                        {
                            if (text.Length + 6 >= maxLength)
                            {
                                num2 = 3;
                            }
                            else
                            {
                                num2 = maxLength - text.Length - 3;
                            }
                            result = text + array[num].Substring(0, num2) + "...";
                        }
                    }
                    else
                    {
                        if (array.GetLength(0) == 2)
                        {
                            result = text + "...\\" + array[1];
                        }
                        else
                        {
                            var num2 = 0;
                            var num3 = 0;
                            for (var i = 0; i < num; i++)
                            {
                                if (array[i].Length > num2)
                                {
                                    num3 = i;
                                    num2 = array[i].Length;
                                }
                            }
                            var j = pathname.Length - num2 + 3;
                            var num4 = num3 + 1;
                            while (j > maxLength)
                            {
                                if (num3 > 0)
                                {
                                    j -= array[--num3].Length - 1;
                                }
                                if (j <= maxLength)
                                {
                                    break;
                                }
                                if (num4 < num)
                                {
                                    j -= array[++num4].Length - 1;
                                }
                                if (num3 == 0 && num4 == num)
                                {
                                    break;
                                }
                            }
                            for (var i = 0; i < num3; i++)
                            {
                                text = text + array[i] + '\\';
                            }
                            text += "...\\";
                            for (var i = num4; i < num; i++)
                            {
                                text = text + array[i] + '\\';
                            }
                            result = text + array[num];
                        }
                    }
                }
            }
            return result;
        }

        private void LoadRecentFiles() => _recentFiles = LoadRecentFilesCore();

        private List<RecentFile> LoadRecentFilesCore()
        {
            var recentFiles = RecentFiles;
            var list = new List<RecentFile>(recentFiles.Count);
            var i = 0;
            list.AddRange(
                from filepath in recentFiles
                select new RecentFile(i++, filepath));
            return list;
        }

        private void MenuItemClick(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            OnMenuClick(menuItem);
        }

        protected virtual void OnMenuClick(MenuItem menuItem)
        {
            var filepath = GetFilepath(menuItem);
            if (!string.IsNullOrEmpty(filepath))
            {
                if (!Global.DoesDirectoryExist(filepath))
                {
                    PromptForDelete(filepath);
                }
                else
                {
                    if (File.Exists(filepath))
                    {
                        var instance = Ioc.Default.GetRequiredService<MainViewModel>();
                        instance.Open(filepath);
                    }
                    else
                    {
                        PromptForDelete(filepath);
                    }
                }
            }
        }

        private void PromptForDelete(string filepath)
        {
            var messageBoxResult =
                MessageBox.Show(
                    string.Format("{0} is not accessible. Would you like to remove from the recent file list?", filepath),
                    "File Doesnt Exist", MessageBoxButton.YesNo, MessageBoxImage.Hand);
            if (messageBoxResult.Equals(MessageBoxResult.Yes))
            {
                RemoveFile(filepath);
            }
        }

        private string GetFilepath(MenuItem menuItem)
        {
            string result;
            using (var enumerator = (
                from r in _recentFiles
                where r.MenuItem.Equals(menuItem)
                select r).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    result = current.Filepath;
                    return result;
                }
            }
            result = string.Empty;
            return result;
        }

        private static class ApplicationAttributes
        {
            private static readonly Assembly Assembly;
            private static readonly AssemblyTitleAttribute _Title;
            private static readonly AssemblyCompanyAttribute _Company;
            private static readonly AssemblyCopyrightAttribute _Copyright;
            private static readonly AssemblyProductAttribute _Product;
            private static readonly Version _Version;

            static ApplicationAttributes()
            {
                try
                {
                    Title = string.Empty;
                    CompanyName = string.Empty;
                    Copyright = string.Empty;
                    ProductName = string.Empty;
                    Version = string.Empty;
                    Assembly = Assembly.GetEntryAssembly();
                    if (Assembly != null)
                    {
                        var customAttributes = Assembly.GetCustomAttributes(false);
                        var array = customAttributes;
                        foreach (var obj in array)
                        {
                            var type = obj.GetType();
                            if (type == typeof (AssemblyTitleAttribute))
                            {
                                _Title = (AssemblyTitleAttribute) obj;
                            }
                            if (type == typeof (AssemblyCompanyAttribute))
                            {
                                _Company = (AssemblyCompanyAttribute) obj;
                            }
                            if (type == typeof (AssemblyCopyrightAttribute))
                            {
                                _Copyright = (AssemblyCopyrightAttribute) obj;
                            }
                            if (type == typeof (AssemblyProductAttribute))
                            {
                                _Product = (AssemblyProductAttribute) obj;
                            }
                        }
                        _Version = Assembly.GetName().Version;
                    }
                    if (_Title != null)
                    {
                        Title = _Title.Title;
                    }
                    if (_Company != null)
                    {
                        CompanyName = _Company.Company;
                    }
                    if (_Copyright != null)
                    {
                        Copyright = _Copyright.Copyright;
                    }
                    if (_Product != null)
                    {
                        ProductName = _Product.Product;
                    }
                    if (_Version != null)
                    {
                        Version = _Version.ToString();
                    }
                }
                catch (Exception ex)
                {
                    var msg = new ErrorMessage("RecentFileList.ApplicationAttributes", ex, MessageType.Error);
                    WeakReferenceMessenger.Default.Send<IMessage>(msg);
                }
            }

            public static string CompanyName { get; private set; }
            public static string ProductName { get; private set; }
            private static string Copyright { get; set; }
            private static string Title { get; set; }
            private static string Version { get; set; }
        }

        public interface IPersist
        {
            List<string> RecentFiles(int max);
            void InsertFile(string filepath, int max);
            void RemoveFile(string filepath, int max);
        }

        public class MenuClickEventArgs : EventArgs
        {
            public MenuClickEventArgs(string filepath)
            {
                Filepath = filepath;
            }

            public string Filepath { get; private set; }
        }

        private class RecentFile
        {
            public readonly string Filepath = "";
            public readonly int Number;
            public MenuItem MenuItem;

            public RecentFile(int number, string filepath)
            {
                Number = number;
                Filepath = filepath;
            }

            public string DisplayPath
            {
                get
                {
                    var directoryName = Path.GetDirectoryName(Filepath);
                    var path = GlobalOptions.Instance.Options.FileOptions.ShowFullName
                        ? Path.GetFileName(Filepath)
                        : Path.GetFileNameWithoutExtension(Filepath);
                    return Path.Combine(directoryName, path);
                }
            }
        }

        [Localizable(false)]
        private class RegistryPersister : IPersist
        {
            public RegistryPersister()
            {
                RegistryKey = string.Concat(new[]
                {
                    "Software\\",
                    ApplicationAttributes.CompanyName,
                    "\\",
                    ApplicationAttributes.ProductName,
                    "\\RecentFileList"
                });
            }

            public RegistryPersister(string key)
            {
                RegistryKey = key;
            }

            private string RegistryKey { get; set; }

            public List<string> RecentFiles(int max)
            {
                var registryKey = Registry.CurrentUser.OpenSubKey(RegistryKey) ??
                                          Registry.CurrentUser.CreateSubKey(RegistryKey);
                var list = new List<string>(max);
                for (var i = 0; i < max; i++)
                {
                    if (registryKey != null)
                    {
                        var text = (string) registryKey.GetValue(Key(i));
                        if (string.IsNullOrEmpty(text))
                        {
                            break;
                        }
                        list.Add(text);
                    }
                }
                return list;
            }

            public void InsertFile(string filepath, int max)
            {
                var registryKey = Registry.CurrentUser.OpenSubKey(RegistryKey);
                if (registryKey == null)
                {
                    Registry.CurrentUser.CreateSubKey(RegistryKey);
                }
                registryKey = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
                RemoveFile(filepath, max);
                for (var i = max - 2; i >= 0; i--)
                {
                    var name = Key(i);
                    var name2 = Key(i + 1);
                    if (registryKey != null)
                    {
                        var value = registryKey.GetValue(name);
                        if (value != null)
                        {
                            registryKey.SetValue(name2, value);
                        }
                    }
                }
                if (registryKey != null)
                {
                    registryKey.SetValue(Key(0), filepath);
                }
            }

            public void RemoveFile(string filepath, int max)
            {
                var registryKey = Registry.CurrentUser.OpenSubKey(RegistryKey);
                if (registryKey != null)
                {
                    for (var i = 0; i < max; i++)
                    {
                        while (true)
                        {
                            var text = (string) registryKey.GetValue(Key(i));
                            if (text == null || !text.Equals(filepath, StringComparison.CurrentCultureIgnoreCase))
                            {
                                break;
                            }
                            RemoveFile(i, max);
                        }
                    }
                }
            }

            private static string Key(int i) => i.ToString("00");

            private void RemoveFile(int index, int max)
            {
                var registryKey = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
                if (registryKey != null)
                {
                    registryKey.DeleteValue(Key(index), false);
                    for (var i = index; i < max - 1; i++)
                    {
                        var name = Key(i);
                        var name2 = Key(i + 1);
                        var value = registryKey.GetValue(name2);
                        if (value == null)
                        {
                            break;
                        }
                        registryKey.SetValue(name, value);
                        registryKey.DeleteValue(name2);
                    }
                }
            }
        }

        private class XmlPersister : IPersist
        {
            public XmlPersister()
            {
                Filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    ApplicationAttributes.CompanyName + "\\" + ApplicationAttributes.ProductName +
                    "\\RecentFileList.xml");
            }

            public XmlPersister(string filepath)
            {
                Filepath = filepath;
            }

            public XmlPersister(Stream stream)
            {
                Stream = stream;
            }

            private string Filepath { get; set; }
            private Stream Stream { get; set; }

            public List<string> RecentFiles(int max) => Load(max);

            public void InsertFile(string filepath, int max) => Update(filepath, true, max);

            public void RemoveFile(string filepath, int max) => Update(filepath, false, max);

            private void Update(string filepath, bool insert, int max)
            {
                var list = Load(max);
                var list2 = new List<string>(list.Count + 1);
                if (insert)
                {
                    list2.Add(filepath);
                }
                CopyExcluding(list, filepath, list2, max);
                Save(list2);
            }

            private static void CopyExcluding(IEnumerable<string> source, string exclude, ICollection<string> target,
                int max)
            {
                foreach (var current in
                    from s in source
                    where !string.IsNullOrEmpty(s)
                    where !s.Equals(exclude, StringComparison.OrdinalIgnoreCase)
                    where target.Count < max
                    select s)
                {
                    target.Add(current);
                }
            }

            private SmartStream OpenStream(FileMode mode) => (!string.IsNullOrEmpty(Filepath)) ? new SmartStream(Filepath, mode) : new SmartStream(Stream);

            private List<string> Load(int max)
            {
                var list = new List<string>(max);
                List<string> result;
                using (var memoryStream = new MemoryStream())
                {
                    using (var smartStream = OpenStream(FileMode.OpenOrCreate))
                    {
                        if (smartStream.Stream.Length == 0L)
                        {
                            result = list;
                            return result;
                        }
                        smartStream.Stream.Position = 0L;
                        var array = new byte[1048576];
                        while (true)
                        {
                            var num = smartStream.Stream.Read(array, 0, array.Length);
                            if (num == 0)
                            {
                                break;
                            }
                            memoryStream.Write(array, 0, num);
                        }
                        memoryStream.Position = 0L;
                    }
                    XmlTextReader xmlTextReader = null;
                    try
                    {
                        xmlTextReader = new XmlTextReader(memoryStream);
                        while (xmlTextReader.Read())
                        {
                            var nodeType = xmlTextReader.NodeType;
                            if (nodeType != XmlNodeType.Element)
                            {
                                switch (nodeType)
                                {
                                    case XmlNodeType.Whitespace:
                                    case XmlNodeType.XmlDeclaration:
                                        continue;
                                    case XmlNodeType.EndElement:
                                    {
                                        var name = xmlTextReader.Name;
                                        if (name != null)
                                        {
                                            if (name == "RecentFiles")
                                            {
                                                result = list;
                                                return result;
                                            }
                                        }
                                        Debug.Assert(false);
                                        continue;
                                    }
                                }
                                Debug.Assert(false);
                            }
                            else
                            {
                                var name = xmlTextReader.Name;
                                if (name == null)
                                {
                                    goto IL_13E;
                                }
                                if (!(name == "RecentFiles"))
                                {
                                    if (!(name == "RecentFile"))
                                    {
                                        goto IL_13E;
                                    }
                                    if (list.Count < max)
                                    {
                                        list.Add(xmlTextReader.GetAttribute(0));
                                    }
                                }
                                continue;
                                IL_13E:
                                Debug.Assert(false);
                            }
                        }
                    }
                    finally
                    {
                        if (xmlTextReader != null)
                        {
                            xmlTextReader.Close();
                        }
                    }
                }
                result = list;
                return result;
            }

            private void Save(IEnumerable<string> list)
            {
                using (var memoryStream = new MemoryStream())
                {
                    XmlTextWriter xmlTextWriter = null;
                    try
                    {
                        xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8)
                        {
                            Formatting = Formatting.Indented
                        };
                        xmlTextWriter.WriteStartDocument();
                        xmlTextWriter.WriteStartElement("RecentFiles");
                        foreach (var current in list)
                        {
                            xmlTextWriter.WriteStartElement("RecentFile");
                            xmlTextWriter.WriteAttributeString("Filepath", current);
                            xmlTextWriter.WriteEndElement();
                        }
                        xmlTextWriter.WriteEndElement();
                        xmlTextWriter.WriteEndDocument();
                        xmlTextWriter.Flush();
                        using (var smartStream = OpenStream(FileMode.Create))
                        {
                            smartStream.Stream.SetLength(0L);
                            memoryStream.Position = 0L;
                            var array = new byte[1048576];
                            while (true)
                            {
                                var num = memoryStream.Read(array, 0, array.Length);
                                if (num == 0)
                                {
                                    break;
                                }
                                smartStream.Stream.Write(array, 0, num);
                            }
                        }
                    }
                    finally
                    {
                        if (xmlTextWriter != null)
                        {
                            xmlTextWriter.Close();
                        }
                    }
                }
            }

            private sealed class SmartStream : IDisposable
            {
                private readonly bool _isStreamOwned = true;

                public SmartStream(string filepath, FileMode mode)
                {
                    _isStreamOwned = true;
                    var directoryName = Path.GetDirectoryName(filepath);
                    Directory.CreateDirectory(directoryName);
                    Stream = File.Open(filepath, mode);
                }

                public SmartStream(Stream stream)
                {
                    _isStreamOwned = false;
                    Stream = stream;
                }

                public Stream Stream { get; private set; }

                public void Dispose()
                {
                    if (_isStreamOwned && Stream != null)
                    {
                        Stream.Dispose();
                    }
                    Stream = null;
                }

                public static implicit operator Stream(SmartStream me)
                {
                    return me.Stream;
                }
            }
        }
    }
}