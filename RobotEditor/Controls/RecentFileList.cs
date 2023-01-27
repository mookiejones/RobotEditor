using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Win32;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Messages;
using RobotEditor.ViewModel;
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
                    arg_15_0 = _instance = new RecentFileList();
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
                foreach (RecentFile current in
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
                    int num = FileMenu.Items.IndexOf(this);
                    foreach (RecentFile current in _recentFiles)
                    {
                        string menuItemText = GetMenuItemText(current.Number + 1, current.Filepath, current.DisplayPath);
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
            GetMenuItemTextDelegate getMenuItemTextHandler = GetMenuItemTextHandler;
            string result;
            if (getMenuItemTextHandler != null)
            {
                result = getMenuItemTextHandler(index, filepath);
            }
            else
            {
                string format = (index < 10) ? MenuItemFormatOneToNine : MenuItemFormatTenPlus;
                string arg = ShortenPathname(displaypath, MaxPathLength);
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
                string text = Path.GetPathRoot(pathname);
                if (text.Length > 3)
                {
                    text += Path.DirectorySeparatorChar;
                }
                string[] array = pathname.Substring(text.Length).Split(new[]
                {
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                });
                int num = array.GetLength(0) - 1;
                if (array.GetLength(0) == 1)
                {
                    if (array[0].Length > 5)
                    {
                        result = text.Length + 6 >= maxLength ? text + array[0].Substring(0, 3) + "..." : pathname.Substring(0, maxLength - 3) + "...";
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
                        int num2 = array[num].Length;
                        if (num2 < 6)
                        {
                            result = text + array[num];
                        }
                        else
                        {
                            num2 = text.Length + 6 >= maxLength ? 3 : maxLength - text.Length - 3;
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
                            int num2 = 0;
                            int num3 = 0;
                            for (int i = 0; i < num; i++)
                            {
                                if (array[i].Length > num2)
                                {
                                    num3 = i;
                                    num2 = array[i].Length;
                                }
                            }
                            int j = pathname.Length - num2 + 3;
                            int num4 = num3 + 1;
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
                            for (int i = 0; i < num3; i++)
                            {
                                text = text + array[i] + '\\';
                            }
                            text += "...\\";
                            for (int i = num4; i < num; i++)
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
            List<string> recentFiles = RecentFiles;
            List<RecentFile> list = new List<RecentFile>(recentFiles.Count);
            int i = 0;
            list.AddRange(
                from filepath in recentFiles
                select new RecentFile(i++, filepath));
            return list;
        }

        private void MenuItemClick(object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            OnMenuClick(menuItem);
        }

        protected virtual void OnMenuClick(MenuItem menuItem)
        {
            string filepath = GetFilepath(menuItem);
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
                        MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
                        _ = instance.Open(filepath);
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
            MessageBoxResult messageBoxResult =
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
            using (IEnumerator<RecentFile> enumerator = (
                from r in _recentFiles
                where r.MenuItem.Equals(menuItem)
                select r).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    RecentFile current = enumerator.Current;
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
                        object[] customAttributes = Assembly.GetCustomAttributes(false);
                        object[] array = customAttributes;
                        foreach (object obj in array)
                        {
                            Type type = obj.GetType();
                            if (type == typeof(AssemblyTitleAttribute))
                            {
                                _Title = (AssemblyTitleAttribute)obj;
                            }
                            if (type == typeof(AssemblyCompanyAttribute))
                            {
                                _Company = (AssemblyCompanyAttribute)obj;
                            }
                            if (type == typeof(AssemblyCopyrightAttribute))
                            {
                                _Copyright = (AssemblyCopyrightAttribute)obj;
                            }
                            if (type == typeof(AssemblyProductAttribute))
                            {
                                _Product = (AssemblyProductAttribute)obj;
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
                    ErrorMessage msg = new ErrorMessage("RecentFileList.ApplicationAttributes", ex, MessageType.Error);
                    _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
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
                    string directoryName = Path.GetDirectoryName(Filepath);
                    string path = GlobalOptions.Instance.Options.FileOptions.ShowFullName
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
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistryKey) ??
                                          Registry.CurrentUser.CreateSubKey(RegistryKey);
                List<string> list = new List<string>(max);
                for (int i = 0; i < max; i++)
                {
                    if (registryKey != null)
                    {
                        string text = (string)registryKey.GetValue(Key(i));
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
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistryKey);
                if (registryKey == null)
                {
                    _ = Registry.CurrentUser.CreateSubKey(RegistryKey);
                }
                registryKey = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
                RemoveFile(filepath, max);
                for (int i = max - 2; i >= 0; i--)
                {
                    string name = Key(i);
                    string name2 = Key(i + 1);
                    if (registryKey != null)
                    {
                        object value = registryKey.GetValue(name);
                        if (value != null)
                        {
                            registryKey.SetValue(name2, value);
                        }
                    }
                }
                registryKey?.SetValue(Key(0), filepath);
            }

            public void RemoveFile(string filepath, int max)
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistryKey);
                if (registryKey != null)
                {
                    for (int i = 0; i < max; i++)
                    {
                        while (true)
                        {
                            string text = (string)registryKey.GetValue(Key(i));
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
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
                if (registryKey != null)
                {
                    registryKey.DeleteValue(Key(index), false);
                    for (int i = index; i < max - 1; i++)
                    {
                        string name = Key(i);
                        string name2 = Key(i + 1);
                        object value = registryKey.GetValue(name2);
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
                List<string> list = Load(max);
                List<string> list2 = new List<string>(list.Count + 1);
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
                foreach (string current in
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
                List<string> list = new List<string>(max);
                List<string> result;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (SmartStream smartStream = OpenStream(FileMode.OpenOrCreate))
                    {
                        if (smartStream.Stream.Length == 0L)
                        {
                            result = list;
                            return result;
                        }
                        smartStream.Stream.Position = 0L;
                        byte[] array = new byte[1048576];
                        while (true)
                        {
                            int num = smartStream.Stream.Read(array, 0, array.Length);
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
                            XmlNodeType nodeType = xmlTextReader.NodeType;
                            if (nodeType != XmlNodeType.Element)
                            {
                                switch (nodeType)
                                {
                                    case XmlNodeType.Whitespace:
                                    case XmlNodeType.XmlDeclaration:
                                        continue;
                                    case XmlNodeType.EndElement:
                                        {
                                            string name = xmlTextReader.Name;
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
                                string name = xmlTextReader.Name;
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
                        xmlTextReader?.Close();
                    }
                }
                result = list;
                return result;
            }

            private void Save(IEnumerable<string> list)
            {
                using (MemoryStream memoryStream = new MemoryStream())
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
                        foreach (string current in list)
                        {
                            xmlTextWriter.WriteStartElement("RecentFile");
                            xmlTextWriter.WriteAttributeString("Filepath", current);
                            xmlTextWriter.WriteEndElement();
                        }
                        xmlTextWriter.WriteEndElement();
                        xmlTextWriter.WriteEndDocument();
                        xmlTextWriter.Flush();
                        using (SmartStream smartStream = OpenStream(FileMode.Create))
                        {
                            smartStream.Stream.SetLength(0L);
                            memoryStream.Position = 0L;
                            byte[] array = new byte[1048576];
                            while (true)
                            {
                                int num = memoryStream.Read(array, 0, array.Length);
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
                        xmlTextWriter?.Close();
                    }
                }
            }

            private sealed class SmartStream : IDisposable
            {
                private readonly bool _isStreamOwned = true;

                public SmartStream(string filepath, FileMode mode)
                {
                    _isStreamOwned = true;
                    string directoryName = Path.GetDirectoryName(filepath);
                    _ = Directory.CreateDirectory(directoryName);
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