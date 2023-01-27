using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using Ionic.Zip;

namespace RobotEditor.ViewModel
{
    public sealed class IOViewModel :  ObservableRecipient
    {
        private static OleDbConnection _oleDbConnection;
        private readonly List<Item> _anin = new List<Item>();
        private readonly List<Item> _anout = new List<Item>();
        private readonly List<Item> _counter = new List<Item>();
        private readonly List<Item> _cycflags = new List<Item>();
        private readonly List<Item> _flags = new List<Item>();
        private readonly List<Item> _inputs = new List<Item>();
        private readonly List<Item> _outputs = new List<Item>();
        private readonly ReadOnlyCollection<Item> _readonlyAnIn = null;
        private readonly ReadOnlyCollection<Item> _readonlyAnOut = null;
        private readonly ReadOnlyCollection<Item> _readonlyCounter = null;
        private readonly ReadOnlyCollection<Item> _readonlyCycFlags = null;
        private readonly ReadOnlyCollection<Item> _readonlyFlags = null;
        private readonly ReadOnlyCollection<Item> _readonlyOutputs = null;
        private readonly ReadOnlyObservableCollection<DirectoryInfo> _readonlyRoot = null;
        private readonly ReadOnlyCollection<Item> _readonlyTimer = null;
        private readonly ReadOnlyCollection<Item> _readonlyinputs = null;
        private readonly ObservableCollection<DirectoryInfo> _root = new ObservableCollection<DirectoryInfo>();
        private readonly List<Item> _timer = new List<Item>();
        private string _archivePath = " ";
        private string _buffersize = string.Empty;
        private Visibility _counterVisibility = Visibility.Collapsed;
        private Visibility _cyclicFlagVisibility = Visibility.Collapsed;
        private string _database = string.Empty;
        private string _databaseText = string.Empty;
        private string _filecount = string.Empty;
        private Visibility _flagVisibility = Visibility.Collapsed;
        private InfoFile _info = new InfoFile();
        private string _languageText = string.Empty;
        private DirectoryInfo _rootpath;
        private Visibility _timerVisibility = Visibility.Collapsed;

        public IOViewModel(string filename)
        {
            DataBaseFile = filename;
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += _backgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();
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

        public string DataBaseFile { get; set; }

        public string DataBase { get =>_database; set=>SetProperty(ref _database,value); }

        public string InfoFile { get; set; }

        public ReadOnlyObservableCollection<DirectoryInfo> Root => _readonlyRoot ?? new ReadOnlyObservableCollection<DirectoryInfo>(_root);

        public DirectoryInfo RootPath { get =>_rootpath; set=>SetProperty(ref _rootpath,value); }

        public string LanguageText { get =>_languageText; set=>SetProperty(ref _languageText,value); }

        public string DatabaseText { get =>_databaseText; set=>SetProperty(ref _databaseText,value); }

        public ReadOnlyCollection<Item> Inputs => _readonlyinputs ?? new ReadOnlyCollection<Item>(_inputs);

        public ReadOnlyCollection<Item> Outputs => _readonlyOutputs ?? new ReadOnlyCollection<Item>(_outputs);

        public ReadOnlyCollection<Item> AnIn => _readonlyAnIn ?? new ReadOnlyCollection<Item>(_anin);

        public ReadOnlyCollection<Item> AnOut => _readonlyAnOut ?? new ReadOnlyCollection<Item>(_anout);

        public ReadOnlyCollection<Item> Timer => _readonlyTimer ?? new ReadOnlyCollection<Item>(_timer);

        public ReadOnlyCollection<Item> Flags => _readonlyFlags ?? new ReadOnlyCollection<Item>(_flags);

        public ReadOnlyCollection<Item> CycFlags => _readonlyCycFlags ?? new ReadOnlyCollection<Item>(_cycflags);

        public ReadOnlyCollection<Item> Counter => _readonlyCounter ?? new ReadOnlyCollection<Item>(_counter);

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (string.IsNullOrEmpty(DataBaseFile))
                return;
            GetSignals();
            GetTimers();
            GetAllLangtextFromDatabase();
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnPropertyChanged(nameof(Inputs));
            OnPropertyChanged(nameof(Outputs));
            OnPropertyChanged(nameof(AnIn));
            OnPropertyChanged(nameof(AnOut));
            OnPropertyChanged(nameof(Counter));
            OnPropertyChanged(nameof(Flags));
            OnPropertyChanged(nameof(Timer));
        }

        private OleDbConnection GetDBConnection()
        {
            var connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
            try
            {
                if (_oleDbConnection == null)
                {
                    _oleDbConnection = new OleDbConnection(connectionString);
                }
            }
            catch (Exception ex)
            {
                _oleDbConnection = null;
                Console.WriteLine(ex);
            }
            return _oleDbConnection;
        }

        private IEnumerable<Item> getItems(string command, string itemType, int idx)
        {
            var result = new List<Item>();
            var dbConnection = GetDBConnection();
            if (dbConnection != null)
            {
                if (dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();
                using (var cmd = new OleDbCommand(command, dbConnection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader != null && reader.Read())
                        {
                            var text = reader.GetValue(0).ToString();
                            var item =
                                new Item(String.Format(itemType, text.Substring(idx)), reader.GetValue(1).ToString());
                            result.Add(item);
                        }
                    }
                }
            }

            return result;
        }

        private void GetSignals()
        {
            if (File.Exists(DataBaseFile))
            {
                var dBConnection = GetDBConnection();

                if (dBConnection == null)
                {
                    return;
                }
                if (dBConnection.State != ConnectionState.Open)
                    dBConnection.Open();
                using (
                    var oleDbCommand =
                        new OleDbCommand(
                            "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'IO')",
                            dBConnection))
                {
                    using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                    {
                        while (oleDbDataReader != null && oleDbDataReader.Read())
                        {
                            var text2 = oleDbDataReader.GetValue(0).ToString();
                            var text3 = oleDbDataReader.GetValue(1).ToString();
                            var description = (text3 == "|EMPTY|") ? "Spare" : text3;
                            var text4 = text2.Substring(0, text2.IndexOf("_", StringComparison.Ordinal));
                            if (text4 != null)
                            {
                                if (!(text4 == "IN"))
                                {
                                    if (!(text4 == "OUT"))
                                    {
                                        if (!(text4 == "ANIN"))
                                        {
                                            if (text4 == "ANOUT")
                                            {
                                                var item = new Item(string.Format("$ANOUT[{0}]", text2.Substring(6)),
                                                    description);
                                                _anout.Add(item);
                                                LanguageText = LanguageText + item + "\r\n";
                                            }
                                        }
                                        else
                                        {
                                            var item = new Item(string.Format("$ANIN[{0}]", text2.Substring(5)),
                                                description);
                                            _anin.Add(item);
                                            LanguageText = LanguageText + item + "\r\n";
                                        }
                                    }
                                    else
                                    {
                                        var item = new Item(string.Format("$OUT[{0}]", text2.Substring(4)), description);
                                        if (!_outputs.Contains(item))
                                        {
                                            _outputs.Add(item);
                                        }
                                        LanguageText = LanguageText + item + "\r\n";
                                    }
                                }
                                else
                                {
                                    var item = new Item(string.Format("$IN[{0}]", text2.Substring(3)), description);
                                    _inputs.Add(item);
                                    LanguageText = LanguageText + item + "\r\n";
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

        private void GetFlags()
        {
            var items =
                getItems(
                    "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'FLAG')",
                    "$FLAG[{0}]", 8);
            _flags.AddRange(items);
            FlagVisibility = ((Flags.Count > 0) ? Visibility.Visible : Visibility.Collapsed);
            OnPropertyChanged(nameof(FlagVisibility));
        }

        private void GetTimers()
        {
            var items =
                getItems(
                    "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = 'TIMER')",
                    "$TIMER[{0}]", 9);

//                            var item = new Item(string.Format("$TIMER[{0}]", text.Substring(9)), oleDbDataReader.GetValue(1).ToString());
            _timer.AddRange(items);
            TimerVisibility = ((Timer.Count > 0) ? Visibility.Visible : Visibility.Collapsed);
            OnPropertyChanged(nameof(TimerVisibility));
        }

        private List<Item> GetValues(string cmd, int index)
        {
            var list = new List<Item>();
            var cmdText =
                string.Format(
                    "SELECT Items.KeyString, Messages.[String] FROM (Items INNER JOIN Messages ON Items.Key_id = Messages.Key_id)WHERE (Items.[Module] = '{0}')",
                    cmd);
            var dBConnection = GetDBConnection();
            List<Item> result;
            if (dBConnection == null)
            {
                result = list;
            }
            else
            {
                dBConnection.Open();
                using (var oleDbCommand = new OleDbCommand(cmdText, dBConnection))
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
                result = list;
            }
            return result;
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
            DataBaseFile = openFileDialog.FileName;
            GetSignals();
        }

        private void GetAllLangtextFromDatabase()
        {
            LanguageText = string.Empty;
            if (File.Exists(DataBaseFile))
            {
                var text = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + DataBaseFile + ";";
                var dBConnection = GetDBConnection();
                if (dBConnection != null)
                {
                    dBConnection.Open();
                    using (
                        var oleDbCommand =
                            new OleDbCommand(
                                "SELECT i.keystring, m.string FROM ITEMS i, messages m where i.key_id=m.key_id and m.language_id=99",
                                dBConnection))
                    {
                        using (var oleDbDataReader = oleDbCommand.ExecuteReader())
                        {
                            while (oleDbDataReader != null && oleDbDataReader.Read())
                            {
                                var arg = oleDbDataReader.GetValue(0).ToString();
                                var arg2 = oleDbDataReader.GetValue(1).ToString();
                                DataBase += string.Format("{0} {1}\r\n", arg, arg2);
                            }
                        }
                    }
                }
            }
        }
    }
}