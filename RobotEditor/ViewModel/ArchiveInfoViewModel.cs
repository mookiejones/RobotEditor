using CommunityToolkit.Mvvm.Input;
using Ionic.Zip;
using RobotEditor.Enums;
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
        private readonly string[] _langText;
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

        public Visibility FlagVisibility { get => _flagVisibility; set => SetProperty(ref _flagVisibility, value); }

        public Visibility TimerVisibility { get => _timerVisibility; set => SetProperty(ref _timerVisibility, value); }

        public Visibility CyclicFlagVisibility { get => _cyclicFlagVisibility; set => SetProperty(ref _cyclicFlagVisibility, value); }

        public Visibility CounterVisibility { get => _counterVisibility; set => SetProperty(ref _counterVisibility, value); }

        public InfoFile Info { get => _info; set => SetProperty(ref _info, value); }

        public string DirectoryPath { get; set; }

        public string ArchivePath { get => _archivePath; set => SetProperty(ref _archivePath, value); }

        public string FileCount { get => _filecount; set => SetProperty(ref _filecount, value); }

        public ZipFile ArchiveZip { get; set; }

        public string BufferSize { get => _buffersize; set => SetProperty(ref _buffersize, value); }

        public string DatabaseFile { get; set; }

        public string Database { get => _database; set => SetProperty(ref _database, value); }

        public string InfoFile { get; set; }

        public ReadOnlyObservableCollection<DirectoryInfo> Root => _readonlyRoot ?? new ReadOnlyObservableCollection<DirectoryInfo>(_root);

        private static string StartupPath => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public DirectoryInfo RootPath { get => _rootpath; set => SetProperty(ref _rootpath, value); }

        public string LanguageText { get => _languageText; set => SetProperty(ref _languageText, value); }

        public string DatabaseText { get => _databaseText; set => SetProperty(ref _databaseText, value); }

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
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Archive",
                Filter = "KUKA Archive (*.zip)|*.zip",
                Multiselect = false
            };
            _ = openFileDialog.ShowDialog();
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

        private static bool CheckPathExists(string path)
        {
            bool result;
            if (!Directory.Exists(path))
            {
                result = false;
            }
            else
            {
                DialogResult dialogResult =
                    MessageBox.Show(
                        string.Format("The path of {0} \r\n allready exists. Do you want to Delete the path?", path),
                        "Archive Exists", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button3);
                DialogResult dialogResult2 = dialogResult;
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
            bool flag = CheckPathExists(DirectoryPath);
            if (!flag)
            {
                ArchiveZip.ExtractAll(DirectoryPath);
                _root.Add(new DirectoryInfo(DirectoryPath));
            }
        }

        private void GetAMInfo()
        {
            if (File.Exists(InfoFile))
            {
                string[] source = File.ReadAllLines(InfoFile);
                foreach (string[] current in
                    from f in source
                    select f.Split(new[]
                    {
                        '='
                    })
                    into sp
                    where sp.Length > 0
                    select sp)
                {
                    string text = current[0];
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
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseFile + ";";
            using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
            {
                oleDbConnection.Open();
                using (
                    OleDbCommand oleDbCommand =
                        new OleDbCommand(
                            "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'FLAG')",
                            oleDbConnection))
                {
                    using (OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            string text = oleDbDataReader.GetValue(0).ToString();
                            Item item = new Item(string.Format("$FLAG[{0}]", text.Substring(8)),
                                oleDbDataReader.GetValue(1).ToString());
                            _flags.Add(item);
                        }
                    }
                }
            }
            FlagVisibility = (Flags.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged(nameof(FlagVisibility));
        }

        private void GetTimers()
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseFile + ";";
            using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
            {
                oleDbConnection.Open();
                using (
                    OleDbCommand oleDbCommand =
                        new OleDbCommand(
                            "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'TIMER')",
                            oleDbConnection))
                {
                    using (OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            string text = oleDbDataReader.GetValue(0).ToString();
                            Item item = new Item(string.Format("$TIMER[{0}]", text.Substring(9)),
                                oleDbDataReader.GetValue(1).ToString());
                            _timer.Add(item);
                        }
                    }
                }
            }
            TimerVisibility = (Timer.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged(nameof(TimerVisibility));
        }

        private void GetSignals()
        {
            if (File.Exists(DatabaseFile))
            {
                string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseFile + ";";
                using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
                {
                    oleDbConnection.Open();
                    using (
                        OleDbCommand oleDbCommand =
                            new OleDbCommand(
                                "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'IO')",
                                oleDbConnection))
                    {
                        using (OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
                        {
                            while (oleDbDataReader != null && oleDbDataReader.Read())
                            {
                                string text = oleDbDataReader.GetValue(0).ToString();
                                string text2 = text.Substring(0, text.IndexOf("_", StringComparison.Ordinal));
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
                                                    Item item = new Item(
                                                        string.Format("$ANOUT[{0}]", text.Substring(6)),
                                                        oleDbDataReader.GetValue(1).ToString());
                                                    _anout.Add(item);
                                                    LanguageText = LanguageText + item + "\r\n";
                                                }
                                            }
                                            else
                                            {
                                                Item item = new Item(string.Format("$ANIN[{0}]", text.Substring(5)),
                                                    oleDbDataReader.GetValue(1).ToString());
                                                _anin.Add(item);
                                                LanguageText = LanguageText + item + "\r\n";
                                            }
                                        }
                                        else
                                        {
                                            Item item = new Item(string.Format("$OUT[{0}]", text.Substring(4)),
                                                oleDbDataReader.GetValue(1).ToString());
                                            _outputs.Add(item);
                                            LanguageText = LanguageText + item + "\r\n";
                                        }
                                    }
                                    else
                                    {
                                        Item item = new Item(string.Format("$IN[{0}]", text.Substring(3)),
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
                string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DatabaseFile + ";";
                using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
                {
                    oleDbConnection.Open();
                    using (
                        OleDbCommand oleDbCommand =
                            new OleDbCommand(
                                "SELECT i.keystring, m.string FROM ITEMS i, messages m where i.key_id=m.key_id and m.language_id=99",
                                oleDbConnection))
                    {
                        using (OleDbDataReader oleDbDataReader = oleDbCommand.ExecuteReader())
                        {
                            while (oleDbDataReader != null && oleDbDataReader.Read())
                            {
                                string arg = oleDbDataReader.GetValue(0).ToString();
                                string arg2 = oleDbDataReader.GetValue(1).ToString();
                                Database += string.Format("{0} {1}\r\n", arg, arg2);
                            }
                        }
                    }
                }
            }
        }

        private void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            OpenFileDialog openFileDialog2 = openFileDialog;
            openFileDialog2.Filter = "longtext (*.mdb)|*.mdb";
            openFileDialog2.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _dbFile = openFileDialog.FileName;
                Database = _dbFile;
                InitGrids();
            }
        }

        private void InitGrids()
        {
            _progress.Maximum = 5403;
            _progress.Value = 0;
            _progress.IsVisible = true;
            int num = 1;
            checked
            {
                for (int i = 1; i < 4096; i++)
                {
                    _inputs.Add(new Item(string.Format("$IN[{0}]", i), string.Empty));
                    _outputs.Add(new Item(string.Format("$OUT[{0}]", i), string.Empty));
                    _progress.Value++;
                    num++;
                }
                for (int i = 1; i < 32; i++)
                {
                    _anin.Add(new Item(string.Format("$ANIN[{0}]", i), string.Empty));
                    _anout.Add(new Item(string.Format("$ANOUT[{0}]", i), string.Empty));
                    _progress.Value++;
                }
                for (int i = 1; i < 20; i++)
                {
                    _timer.Add(new Item(string.Format("$TIMER[{0}]", i), string.Empty));
                    _counter.Add(new Item(string.Format("$COUNT_I[{0}]", i), string.Empty));
                    _progress.Value++;
                }
                for (int i = 1; i < 999; i++)
                {
                    _flags.Add(new Item(string.Format("$FLAG[{0}]", i), string.Empty));
                    _progress.Value++;
                }
                for (int i = 1; i < 256; i++)
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
                string[] directories = Directory.GetDirectories(dir);
                foreach (string text in directories)
                {
                    string[] files = Directory.GetFiles(text);
                    foreach (string text2 in files)
                    {
                        string fileName = Path.GetFileName(text2);
                        if (fileName != null)
                        {
                            string text3 = fileName.ToLower();
                            Console.WriteLine(text3);
                            string text4 = text3;
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