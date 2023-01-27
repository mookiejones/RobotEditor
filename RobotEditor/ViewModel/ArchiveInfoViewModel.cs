using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Ionic.Zip;
using RobotEditor.Enums;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace RobotEditor.ViewModel
{
    public sealed class ArchiveInfoViewModel : ToolViewModel
    {
// ReSharper disable once ConvertToConstant.Local
        private readonly ObservableCollection<Item> _anin = new ObservableCollection<Item>();
        private readonly ObservableCollection<Item> _anout = new ObservableCollection<Item>();
        private readonly ObservableCollection<Item> _counter = new ObservableCollection<Item>();
        private readonly ObservableCollection<Item> _cycflags = new ObservableCollection<Item>();
        private readonly ObservableCollection<Item> _flags = new ObservableCollection<Item>();
        private readonly bool _isKRC2 = true;
        private readonly ObservableCollection<Item> _outputs = new ObservableCollection<Item>();
        private readonly ProgressBarViewModel _progress = new ProgressBarViewModel();
        private readonly ReadOnlyObservableCollection<Item> _readonlyAnIn = null;
        private readonly ReadOnlyObservableCollection<Item> _readonlyAnOut = null;
        private readonly ReadOnlyObservableCollection<Item> _readonlyCounter = null;
        private readonly ReadOnlyObservableCollection<Item> _readonlyCycFlags = null;
        private readonly ReadOnlyObservableCollection<Item> _readonlyFlags = null;
        private readonly ReadOnlyObservableCollection<Item> _readonlyOutputs = null;
        private readonly ReadOnlyObservableCollection<DirectoryInfo> _readonlyRoot = null;
        private readonly ReadOnlyObservableCollection<Item> _readonlyTimer = null;
        private readonly ReadOnlyObservableCollection<Item> _readonlyinputs = null;
        private readonly ObservableCollection<DirectoryInfo> _root = new ObservableCollection<DirectoryInfo>();
        private readonly ObservableCollection<Item> _timer = new ObservableCollection<Item>();
        private string _archivePath = " ";
        private string _buffersize = string.Empty;
        private Visibility _counterVisibility = Visibility.Collapsed;
        private Visibility _cyclicFlagVisibility = Visibility.Collapsed;
        private string _database = string.Empty;
        private string _databaseText = string.Empty;
        private string _dbFile = string.Empty;
        private string _filecount = string.Empty;
        private Visibility _flagVisibility = Visibility.Collapsed;
        private RelayCommand _importCommand;
        private InfoFile _info = new InfoFile();
        public ObservableCollection<Item> _inputs = new ObservableCollection<Item>();
        private string[] _langText;
        private string _languageText = string.Empty;
        private RelayCommand _loadCommand;
        private RelayCommand _openCommand;
        private DirectoryInfo _rootpath;
        private Visibility _timerVisibility = Visibility.Collapsed;

        public ArchiveInfoViewModel()
            : base("ArchiveInfoViewModel")
        {
            _root = new ObservableCollection<DirectoryInfo>();
            OnPropertyChanged(nameof(DigInVisibility));
            OnPropertyChanged(nameof(DigOutVisibility));
            OnPropertyChanged(nameof(AnInVisibility));
            OnPropertyChanged(nameof(AnOutVisibility));
            DefaultPane = DefaultToolPane.Right;
            base.Width = 250;
            base.Height = 600;
        }

        public Visibility DigInVisibility => (Inputs.Count > 0) ? Visibility.Visible : Visibility.Hidden;

        public Visibility DigOutVisibility => (Outputs.Count > 0) ? Visibility.Visible : Visibility.Hidden;

        public Visibility AnInVisibility => (AnIn.Count > 0) ? Visibility.Visible : Visibility.Hidden;

        public Visibility AnOutVisibility => (AnOut.Count > 0) ? Visibility.Visible : Visibility.Hidden;

        public Visibility DigitalVisibility => (DigInVisibility == Visibility.Visible && DigOutVisibility == Visibility.Visible)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

        public Visibility AnalogVisibility => (AnOutVisibility == Visibility.Visible || AnInVisibility == Visibility.Visible)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

        public Visibility FlagVisibility { get =>_flagVisibility; set=>SetProperty(ref _flagVisibility,value); }

        public Visibility TimerVisibility { get =>_timerVisibility; set=>SetProperty(ref _timerVisibility,value); }

        public Visibility CyclicFlagVisibility { get =>_cyclicFlagVisibility; set=>SetProperty(ref _cyclicFlagVisibility,value); }

        public Visibility CounterVisibility { get =>_counterVisibility; set=>SetProperty(ref _counterVisibility,value); }

        public InfoFile Info { get =>_info; set=>SetProperty(ref _info,value); }

        public string DirectoryPath { get; set; }

        public string ArchivePath { get =>_archivePath; set=>SetProperty(ref _archivePath,value); }

        public string FileCount { get =>_filecount; set=>SetProperty(ref _filecount,value); }

        public ZipFile ArchiveZip { get; set; }

        public string BufferSize { get =>_buffersize; set=>SetProperty(ref _buffersize,value); }

        public string DatabaseFile { get; set; }

        public string Database { get =>_database; set=>SetProperty(ref _database,value); }

        public string InfoFile { get; set; }

        public ReadOnlyObservableCollection<DirectoryInfo> Root => _readonlyRoot ?? new ReadOnlyObservableCollection<DirectoryInfo>(_root);

        private static string StartupPath => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public DirectoryInfo RootPath { get =>_rootpath; set=>SetProperty(ref _rootpath,value); }

        public string LanguageText { get =>_languageText; set=>SetProperty(ref _languageText,value); }

        public string DatabaseText { get =>_databaseText; set=>SetProperty(ref _databaseText,value); }

        public ReadOnlyObservableCollection<Item> Inputs => _readonlyinputs ?? new ReadOnlyObservableCollection<Item>(_inputs);

        public ReadOnlyObservableCollection<Item> Outputs => _readonlyOutputs ?? new ReadOnlyObservableCollection<Item>(_outputs);

        public ReadOnlyObservableCollection<Item> AnIn => _readonlyAnIn ?? new ReadOnlyObservableCollection<Item>(_anin);

        public ReadOnlyObservableCollection<Item> AnOut => _readonlyAnOut ?? new ReadOnlyObservableCollection<Item>(_anout);

        public ReadOnlyObservableCollection<Item> Timer => _readonlyTimer ?? new ReadOnlyObservableCollection<Item>(_timer);

        public ReadOnlyObservableCollection<Item> Flags => _readonlyFlags ?? new ReadOnlyObservableCollection<Item>(_flags);

        public ReadOnlyObservableCollection<Item> CycFlags => _readonlyCycFlags ?? new ReadOnlyObservableCollection<Item>(_cycflags);

        public ReadOnlyObservableCollection<Item> Counter => _readonlyCounter ?? new ReadOnlyObservableCollection<Item>(_counter);

        public ICommand OpenCommand => _openCommand ?? (_openCommand = new RelayCommand(Open));

        public ICommand ImportCommand => _importCommand ?? (_importCommand = new RelayCommand(Import));

        public ICommand LoadCommand => _loadCommand ?? (_loadCommand = new RelayCommand(GetSignals));

        private void Import()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Archive",
                Filter = "KUKA Archive (*.zip)|*.zip",
                Multiselect = false
            };
            openFileDialog.ShowDialog();
            _root.Clear();
            if (File.Exists(openFileDialog.FileName))
            {
                ArchivePath = openFileDialog.FileName;
                ArchiveZip = new ZipFile(openFileDialog.FileName);
                OnPropertyChanged(nameof(ArchiveZip));
                GetAllLangtextFromDatabase();
                UnpackZip();
                GetFiles(DirectoryPath);
                GetAMInfo();
                GetSignals();
            }
        }

        private void ReadZip()
        {
            foreach (var current in
                from z in ArchiveZip.EntriesSorted
                where z.IsDirectory
                select z)
            {
                Console.WriteLine(current.FileName);
            }
            foreach (var current2 in
                from e in ArchiveZip.Entries
                where e.IsDirectory
                select e)
            {
                Console.WriteLine(current2.FileName);
            }
            OnPropertyChanged(nameof(Root));
        }

        private static bool CheckPathExists(string path)
        {
            bool result;
            if (!Directory.Exists(path))
            {
                result = false;
            }
            else
            {
                var dialogResult =
                    MessageBox.Show(
                        string.Format("The path of {0} \r\n allready exists. Do you want to Delete the path?", path),
                        "Archive Exists", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button3);
                var dialogResult2 = dialogResult;
                if (dialogResult2 != DialogResult.Cancel)
                {
                    switch (dialogResult2)
                    {
                        case DialogResult.Yes:
                            Directory.Delete(path, true);
                            return false;
                        case DialogResult.No:
                            break;
                        default:
                            return false;
                    }
                }
                result = true;
            }
            return result;
        }

        private void UnpackZip()
        {
            DirectoryPath = Path.Combine(StartupPath, Path.GetFileNameWithoutExtension(ArchivePath));
            var flag = CheckPathExists(DirectoryPath);
            if (!flag)
            {
                ArchiveZip.ExtractAll(DirectoryPath);
                _root.Add(new DirectoryInfo(DirectoryPath));
            }
        }

        private void GetDirectories()
        {
        }

        private void GetAMInfo()
        {
            if (File.Exists(InfoFile))
            {
                var source = File.ReadAllLines(InfoFile);
                foreach (var current in
                    from f in source
                    select f.Split(new[]
                    {
                        '='
                    })
                    into sp
                    where sp.Length > 0
                    select sp)
                {
                    var text = current[0];
                    switch (text)
                    {
                        case "Name":
                            Info.ArchiveName = current[1];
                            break;
                        case "Config":
                            Info.ArchiveConfigType = current[1];
                            break;
                        case "DiskNo":
                            Info.ArchiveDiskNo = current[1];
                            break;
                        case "ID":
                            Info.ArchiveID = current[1];
                            break;
                        case "Date":
                            Info.ArchiveDate = current[1];
                            break;
                        case "RobName":
                            Info.RobotName = current[1];
                            break;
                        case "IRSerialNr":
                            Info.RobotSerial = current[1];
                            break;
                        case "Version":
                            Info.KSSVersion = current[1];
                            break;
                    }
                }
            }
        }

        private void GetFlags()
        {
            var connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseFile + ";";
            using (var oleDbConnection = new OleDbConnection(connectionString))
            {
                oleDbConnection.Open();
                using (
                    var oleDbCommand =
                        new OleDbCommand(
                            "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'FLAG')",
                            oleDbConnection))
                {
                    using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            var text = oleDbDataReader.GetValue(0).ToString();
                            var item = new Item(string.Format("$FLAG[{0}]", text.Substring(8)),
                                oleDbDataReader.GetValue(1).ToString());
                            _flags.Add(item);
                        }
                    }
                }
            }
            FlagVisibility = ((Flags.Count > 0) ? Visibility.Visible : Visibility.Collapsed);
            OnPropertyChanged(nameof(FlagVisibility));
        }

        private List<Item> GetValues(string cmd, int index)
        {
            var list = new List<Item>();
            var connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseFile + ";";
            var cmdText =
                string.Format(
                    "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = '{0}')",
                    cmd);
            using (var oleDbConnection = new OleDbConnection(connectionString))
            {
                oleDbConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, oleDbConnection))
                {
                    using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            var text = oleDbDataReader.GetValue(0).ToString();
                            var item = new Item(string.Format("${1}[{0}]", text.Substring(index), cmd),
                                oleDbDataReader.GetValue(1).ToString());
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        private void GetTimers()
        {
            var connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseFile + ";";
            using (var oleDbConnection = new OleDbConnection(connectionString))
            {
                oleDbConnection.Open();
                using (
                    var oleDbCommand =
                        new OleDbCommand(
                            "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'TIMER')",
                            oleDbConnection))
                {
                    using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            var text = oleDbDataReader.GetValue(0).ToString();
                            var item = new Item(string.Format("$TIMER[{0}]", text.Substring(9)),
                                oleDbDataReader.GetValue(1).ToString());
                            _timer.Add(item);
                        }
                    }
                }
            }
            TimerVisibility = ((Timer.Count > 0) ? Visibility.Visible : Visibility.Collapsed);
            OnPropertyChanged(nameof(TimerVisibility));
        }

        private void GetSignalsFromDataBase()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Database",
                Filter = "KUKA Connection Files (kuka_con.mdb)|kuka_con.mdb|All files (*.*)|*.*",
                Multiselect = false
            };
            openFileDialog.ShowDialog();
            LanguageText = string.Empty;
            DatabaseFile = openFileDialog.FileName;
            GetSignals();
        }

        private void GetSignals()
        {
            if (File.Exists(DatabaseFile))
            {
                var connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseFile + ";";
                using (var oleDbConnection = new OleDbConnection(connectionString))
                {
                    oleDbConnection.Open();
                    using (
                        var oleDbCommand =
                            new OleDbCommand(
                                "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'IO')",
                                oleDbConnection))
                    {
                        using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                        {
                            while (oleDbDataReader != null && oleDbDataReader.Read())
                            {
                                var text = oleDbDataReader.GetValue(0).ToString();
                                var text2 = text.Substring(0, text.IndexOf("_", StringComparison.Ordinal));
                                if (text2 != null)
                                {
                                    if (!(text2 == "IN"))
                                    {
                                        if (!(text2 == "OUT"))
                                        {
                                            if (!(text2 == "ANIN"))
                                            {
                                                if (text2 == "ANOUT")
                                                {
                                                    var item = new Item(
                                                        string.Format("$ANOUT[{0}]", text.Substring(6)),
                                                        oleDbDataReader.GetValue(1).ToString());
                                                    _anout.Add(item);
                                                    LanguageText = LanguageText + item + "\r\n";
                                                }
                                            }
                                            else
                                            {
                                                var item = new Item(string.Format("$ANIN[{0}]", text.Substring(5)),
                                                    oleDbDataReader.GetValue(1).ToString());
                                                _anin.Add(item);
                                                LanguageText = LanguageText + item + "\r\n";
                                            }
                                        }
                                        else
                                        {
                                            var item = new Item(string.Format("$OUT[{0}]", text.Substring(4)),
                                                oleDbDataReader.GetValue(1).ToString());
                                            _outputs.Add(item);
                                            LanguageText = LanguageText + item + "\r\n";
                                        }
                                    }
                                    else
                                    {
                                        var item = new Item(string.Format("$IN[{0}]", text.Substring(3)),
                                            oleDbDataReader.GetValue(1).ToString());
                                        _inputs.Add(item);
                                        LanguageText = LanguageText + item + "\r\n";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            GetFlags();
            GetTimers();
            GetAllLangtextFromDatabase();
            OnPropertyChanged(nameof(DigInVisibility));
            OnPropertyChanged(nameof(DigOutVisibility));
            OnPropertyChanged(nameof(AnalogVisibility));
            OnPropertyChanged(nameof(DigitalVisibility));
        }

        private void GetAllLangtextFromDatabase()
        {
            LanguageText = string.Empty;
            if (File.Exists(DatabaseFile))
            {
                var connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseFile + ";";
                using (var oleDbConnection = new OleDbConnection(connectionString))
                {
                    oleDbConnection.Open();
                    using (
                        var oleDbCommand =
                            new OleDbCommand(
                                "SELECT i.keystring, m.string FROM ITEMS i, messages m where i.key_id=m.key_id and m.language_id=99",
                                oleDbConnection))
                    {
                        using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                        {
                            while (oleDbDataReader != null && oleDbDataReader.Read())
                            {
                                var arg = oleDbDataReader.GetValue(0).ToString();
                                var arg2 = oleDbDataReader.GetValue(1).ToString();
                                Database += string.Format("{0} {1}\r\n", arg, arg2);
                            }
                        }
                    }
                }
            }
        }

// ReSharper disable once UnusedMember.Local
        private void ImportFile(string sFile, bool bCsv)
        {
            if (File.Exists(sFile))
            {
                var dialogResult = MessageBox.Show("Delete existing long texts?", "Import File",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                }
                var array = File.ReadAllLines(sFile);
                var text = " ";
                if (bCsv)
                {
                    text = ";";
                }
                var progressBarViewModel = new ProgressBarViewModel
                {
                    Maximum = array.Length,
                    Value = 0,
                    IsVisible = true
                };
                Application.DoEvents();
                var array2 = array;
                var array3 = array2;
                foreach (var text2 in array3)
                {
                    checked
                    {
                        if (text2.Contains(text))
                        {
                            var array4 = text2.Split(text.ToCharArray(), 2);
                            if (array4[0].StartsWith("$IN[", StringComparison.CurrentCultureIgnoreCase))
                            {
                                array4[0] = "IN_" + array4[0].Substring(4, array4[0].Length - 5);
                            }
                            else
                            {
                                if (array4[0].StartsWith("$OUT[", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    array4[0] = "OUT_" + array4[0].Substring(5, array4[0].Length - 6);
                                }
                                else
                                {
                                    if (array4[0].StartsWith("$TIMER[", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        array4[0] = "TimerText" + array4[0].Substring(7, array4[0].Length - 8);
                                    }
                                    else
                                    {
                                        if (array4[0].StartsWith("$COUNT_I[", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            array4[0] = "CounterText" + array4[0].Substring(9, array4[0].Length - 10);
                                        }
                                        else
                                        {
                                            if (array4[0].StartsWith("$FLAG[", StringComparison.CurrentCultureIgnoreCase))
                                            {
                                                array4[0] = "FlagText" + array4[0].Substring(6, array4[0].Length - 7);
                                            }
                                            else
                                            {
                                                if (array4[0].StartsWith("$CYC_FLAG[",
                                                    StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    array4[0] = "NoticeText" +
                                                                array4[0].Substring(10, array4[0].Length - 11);
                                                }
                                                else
                                                {
                                                    if (array4[0].StartsWith("$ANIN[",
                                                        StringComparison.CurrentCultureIgnoreCase))
                                                    {
                                                        array4[0] = "ANIN_" +
                                                                    array4[0].Substring(6, array4[0].Length - 7);
                                                    }
                                                    else
                                                    {
                                                        if (array4[0].StartsWith("$ANOUT[",
                                                            StringComparison.CurrentCultureIgnoreCase))
                                                        {
                                                            array4[0] = "ANOUT_" +
                                                                        array4[0].Substring(6, array4[0].Length - 8);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        progressBarViewModel.Value++;
                    }
                }
                progressBarViewModel.IsVisible = false;
                InitGrids();
            }
        }

        private void Open()
        {
            var openFileDialog = new OpenFileDialog();
            var openFileDialog2 = openFileDialog;
            openFileDialog2.Filter = "longtext (*.mdb)|*.mdb";
            openFileDialog2.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _dbFile = openFileDialog.FileName;
                Database = _dbFile;
                InitGrids();
            }
        }

        private void Export(string filename, bool iscsv)
        {
            var contents = "";
            var text = " ";
            _progress.Maximum = 9551;
            _progress.Value = 0;
            _progress.IsVisible = true;
            var text2 = _isKRC2 ? string.Empty : "$";
            var text3 = _isKRC2 ? "_" : "]";
            if (iscsv)
            {
                text = ";";
            }
            checked
            {
                if (_isKRC2)
                {
                    for (var i = 1; i < 4096; i++)
                    {
                        if (!string.IsNullOrEmpty(_langText[i]))
                        {
                            contents = string.Format("{0}IN{1}{2}{3}{4}\r\n", new object[]
                            {
                                text2,
                                text3,
                                i,
                                text,
                                _langText[i]
                            });
                        }
                        _progress.Value++;
                    }
                    for (var i = 1; i < 4096; i++)
                    {
                        if (!string.IsNullOrEmpty(_langText[i]))
                        {
                            contents = string.Format("{0}OUT{1}{2}{3}{4}\r\n", new object[]
                            {
                                text2,
                                text3,
                                i,
                                text,
                                _langText[i]
                            });
                        }
                        _progress.Value++;
                    }
                    for (var i = 1; i < 32; i++)
                    {
                        if (!string.IsNullOrEmpty(_langText[i]))
                        {
                            contents = string.Format("{0}ANIN{1}{2}{3}{4}\r\n", new object[]
                            {
                                text2,
                                text3,
                                i,
                                text,
                                _langText[i]
                            });
                        }
                        _progress.Value++;
                    }
                    for (var i = 1; i < 32; i++)
                    {
                        if (!string.IsNullOrEmpty(_langText[i]))
                        {
                            contents = string.Format("{0}ANOUT{1}{2}{3}{4}\r\n", new object[]
                            {
                                text2,
                                text3,
                                i,
                                text,
                                _langText[i]
                            });
                        }
                        _progress.Value++;
                    }
                    for (var i = 1; i < 20; i++)
                    {
                        if (!string.IsNullOrEmpty(_langText[i]))
                        {
                            contents = string.Format("{0}{1}{2}{3}{4}\r\n", new object[]
                            {
                                _isKRC2 ? "TimerText" : "$Timer[",
                                text3,
                                i,
                                text,
                                _langText[i]
                            });
                        }
                        _progress.Value++;
                    }
                    for (var i = 1; i < 20; i++)
                    {
                        if (!string.IsNullOrEmpty(_langText[i]))
                        {
                            contents = string.Format("{0}{1}{2}{3}{4}\r\n", new object[]
                            {
                                _isKRC2 ? "CounterText" : "$Counter[",
                                text3,
                                i,
                                text,
                                _langText[i]
                            });
                        }
                        _progress.Value++;
                    }
                    for (var i = 1; i < 999; i++)
                    {
                        if (!string.IsNullOrEmpty(_langText[i]))
                        {
                            contents = string.Format("{0}{1}{2}{3}{4}\r\n", new object[]
                            {
                                _isKRC2 ? "FlagText" : "$FLAG[",
                                text3,
                                i,
                                text,
                                _langText[i]
                            });
                        }
                        _progress.Value++;
                    }
                    for (var i = 1; i < 256; i++)
                    {
                        if (!string.IsNullOrEmpty(_langText[i]))
                        {
                            contents = string.Format("{0}{1}{2}{3}{4}\r\n", new object[]
                            {
                                _isKRC2 ? "NoticeText" : "$CycFlag[",
                                text3,
                                i,
                                text,
                                _langText[i]
                            });
                        }
                        _progress.Value++;
                    }
                }
                File.WriteAllText(filename, contents);
                _progress.IsVisible = false;
            }
        }

        private void InitGrids()
        {
            _progress.Maximum = 5403;
            _progress.Value = 0;
            _progress.IsVisible = true;
            var num = 1;
            checked
            {
                for (var i = 1; i < 4096; i++)
                {
                    _inputs.Add(new Item(string.Format("$IN[{0}]", i), string.Empty));
                    _outputs.Add(new Item(string.Format("$OUT[{0}]", i), string.Empty));
                    _progress.Value++;
                    num++;
                }
                for (var i = 1; i < 32; i++)
                {
                    _anin.Add(new Item(string.Format("$ANIN[{0}]", i), string.Empty));
                    _anout.Add(new Item(string.Format("$ANOUT[{0}]", i), string.Empty));
                    _progress.Value++;
                }
                for (var i = 1; i < 20; i++)
                {
                    _timer.Add(new Item(string.Format("$TIMER[{0}]", i), string.Empty));
                    _counter.Add(new Item(string.Format("$COUNT_I[{0}]", i), string.Empty));
                    _progress.Value++;
                }
                for (var i = 1; i < 999; i++)
                {
                    _flags.Add(new Item(string.Format("$FLAG[{0}]", i), string.Empty));
                    _progress.Value++;
                }
                for (var i = 1; i < 256; i++)
                {
                    _cycflags.Add(new Item(string.Format("$CYCFLAG[{0}]", i), string.Empty));
                    _progress.Value++;
                }
                _progress.IsVisible = false;
            }
        }

        private void GetFiles(string dir)
        {
            if (File.Exists(dir + "\\am.ini"))
            {
                InfoFile = dir + "\\am.ini";
            }
            if (File.Exists(dir + "\\C\\KRC\\Data\\kuka_con.mdb"))
            {
                DatabaseFile = dir + "\\C\\KRC\\Data\\kuka_con.mdb";
            }
            if (!File.Exists(InfoFile) || !File.Exists(DatabaseFile))
            {
                var directories = Directory.GetDirectories(dir);
                foreach (var text in directories)
                {
                    var files = Directory.GetFiles(text);
                    foreach (var text2 in files)
                    {
                        var fileName = Path.GetFileName(text2);
                        if (fileName != null)
                        {
                            var text3 = fileName.ToLower();
                            Console.WriteLine(text3);
                            var text4 = text3;
                            if (text4 != null)
                            {
                                if (!(text4 == "am.ini"))
                                {
                                    if (text4 == "kuka_con.mdb")
                                    {
                                        DatabaseFile = text2;
                                    }
                                }
                                else
                                {
                                    InfoFile = text2;
                                }
                            }
                        }
                    }
                    GetFiles(text);
                }
            }
        }
    }
}