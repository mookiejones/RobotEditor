using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Shell;
using AvalonDock.Layout;
using ControlzEx.Theming;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using RobotEditor.Abstract;
using RobotEditor.Classes;
using RobotEditor.Controls;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Languages;
using RobotEditor.Messages;
using RobotEditor.Windows;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;

namespace RobotEditor.ViewModel
{
    /// <summary>
    ///     This class contains properties that the main View can data bind to.
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
    public sealed class MainViewModel :  ObservableRecipient
    {
        private static IEditorDocument _activeEditor;
        private ILayoutUpdateStrategy _layoutInitializer;

        public MainViewModel()
        {
            AddNewFile();
            _tools = new ObservableCollection<ToolViewModel>
            {
                ObjectBrowser,
                MessageView,
                Notes,
                LocalVariables,
                Functions,
                AngleConverter
            };


            WeakReferenceMessenger.Default.Register<WindowMessage>(this, GetMessage);
        }

        private void GetMessage(object sender, WindowMessage obj) => Open(obj.Description);

        public string Title
        {
            get
            {
                var pathname = ActiveEditor.FilePath ?? string.Empty;
                return ShortenPathname(pathname, 100);
            }
        }

        #region Properties

        #region Tools

        private readonly AngleConvertorViewModel _angleConverter = new AngleConvertorViewModel();
        private readonly FunctionViewModel _functions = new FunctionViewModel();
        private readonly LocalVariablesViewModel _localVariables = new LocalVariablesViewModel();
        private readonly MessageViewModel _messageView = new MessageViewModel();
        private readonly NotesViewModel _notes = new NotesViewModel();
        private readonly ObjectBrowserViewModel _objectBrowser = new ObjectBrowserViewModel();
        private readonly IEnumerable<ToolViewModel> _readonlyTools = null;
        private readonly ObservableCollection<ToolViewModel> _tools = new ObservableCollection<ToolViewModel>();

        public ObjectBrowserViewModel ObjectBrowser => _objectBrowser;

        public NotesViewModel Notes => _notes;

        public MessageViewModel MessageView => _messageView;

        public FunctionViewModel Functions => _functions;

        public LocalVariablesViewModel LocalVariables => _localVariables;

        public AngleConvertorViewModel AngleConverter => _angleConverter;

        #endregion

        #region Files

        private readonly ObservableCollection<IEditorDocument> _files = new ObservableCollection<IEditorDocument>();
        private readonly ReadOnlyObservableCollection<IEditorDocument> _readonlyFiles = null;

        public IEnumerable<IEditorDocument> Files => _readonlyFiles ?? new ReadOnlyObservableCollection<IEditorDocument>(_files);

        #endregion

        #region ShowSettings

        /// <summary>
        ///     The <see cref="ShowSettings" /> property's name.
        /// </summary>
        private const string ShowSettingsPropertyName = "ShowSettings";

        private bool _showSettings;

        /// <summary>
        ///     Sets and gets the ShowSettings property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool ShowSettings
        {
            get => _showSettings;

            set
            {
                if (_showSettings == value)
                {
                    return;
                }

                // ReSharper disable once ExplicitCallerInfoArgument

                _showSettings = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(ShowSettingsPropertyName);
            }
        }

        #endregion



        #region CurrentTheme

        /// <summary>
        ///     The <see cref="CurrentTheme" /> property's name.
        /// </summary>
        private const string CurrentThemePropertyName = "CurrentTheme";

        private Theme _currentTheme;

        /// <summary>
        ///     Sets and gets the CurrentTheme property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public Theme CurrentTheme
        {
            get => _currentTheme;

            set
            {
                if (Equals(_currentTheme, value))
                {
                    return;
                }

                // ReSharper disable once ExplicitCallerInfoArgument
                _currentTheme = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(CurrentThemePropertyName);
            }
        }

        #endregion

        #region ShowIO

        /// <summary>
        ///     The <see cref="ShowIO" /> property's name.
        /// </summary>
        private const string ShowIOPropertyName = "ShowIO";

        private bool _showIO;

        /// <summary>
        ///     Sets and gets the ShowIO property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool ShowIO
        {
            get => _showIO;

            set
            {
                if (_showIO == value)
                {
                    return;
                }

                // ReSharper disable once ExplicitCallerInfoArgument

                _showIO = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(ShowIOPropertyName);
            }
        }

        #endregion

        #region EnableIO

        /// <summary>
        ///     The <see cref="EnableIO" /> property's name.
        /// </summary>
        private const string EnableIOPropertyName = "EnableIO";

        private bool _enableIO;

        /// <summary>
        ///     Sets and gets the EnableIO property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool EnableIO
        {
            get => _enableIO;

            set
            {
                if (_enableIO == value)
                {
                    return;
                }

                // ReSharper disable once ExplicitCallerInfoArgument

                _enableIO = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(EnableIOPropertyName);
            }
        }

        #endregion

        public ILayoutUpdateStrategy LayoutStrategy => _layoutInitializer ?? (_layoutInitializer = new LayoutInitializer());

        public IEnumerable<ToolViewModel> Tools => _readonlyTools ?? new ObservableCollection<ToolViewModel>(_tools);


        public IEditorDocument ActiveEditor
        {
            get => _activeEditor;
            set
            {
                if (_activeEditor != value)
                {
                    _activeEditor = value;
                    if (_activeEditor != null)
                        _activeEditor.TextBox.Focus();
                    // ReSharper disable once RedundantArgumentDefaultValue
                    OnPropertyChanged(nameof(ActiveEditor));
                    // ReSharper disable once ExplicitCallerInfoArgument
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        #region IsClosing

        /// <summary>
        ///     The <see cref="IsClosing" /> property's name.
        /// </summary>
        private const string IsClosingPropertyName = "IsClosing";

        private bool _isClosing;

        /// <summary>
        ///     Sets and gets the IsClosing property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool IsClosing
        {
            get => _isClosing;

            set
            {
                if (_isClosing == value)
                {
                    return;
                }

                // ReSharper disable once ExplicitCallerInfoArgument

                _isClosing = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(IsClosingPropertyName);
            }
        }

        #endregion

        #endregion

        #region Commands

        #region CloseCommand

        private RelayCommand<object> _closeCommand;

        /// <summary>
        ///     Gets the CloseCommand.
        /// </summary>
        public RelayCommand<object> CloseCommand => _closeCommand ?? (_closeCommand = new RelayCommand<object>(ExecuteCloseCommand, CanExecuteCloseCommand));



        private void ExecuteCloseCommand(object obj)

        {
            _files.Remove(ActiveEditor);
            
            ActiveEditor.Close();
            ActiveEditor = _files.FirstOrDefault();
           // Close(ActiveEditor);
            OnPropertyChanged(nameof(ActiveEditor));
        }

        private bool CanExecuteCloseCommand(object arg) => true;

        #endregion

        #region ShowIOCommand

        private RelayCommand _showIOCommand;

        /// <summary>
        ///     Gets the ShowIOCommand.
        /// </summary>
        public RelayCommand ShowIOCommand => _showIOCommand
                       ?? (_showIOCommand = new RelayCommand(ExecuteShowIO));

        #endregion

        #region ChangeThemeCommand

        private RelayCommand<object> _changeThemeCommand;

        /// <summary>
        ///     Gets the ChangeThemeCommand.
        /// </summary>
        public RelayCommand<object> ChangeThemeCommand => _changeThemeCommand
                       ?? (_changeThemeCommand = new RelayCommand<object>(
                           ChangeTheme));

        #endregion

        #region ChangeAccentCommand

        private RelayCommand<object> _changeAccentCommand;

        /// <summary>
        ///     Gets the ChangeAccentCommand.
        /// </summary>
        public RelayCommand<object> ChangeAccentCommand => _changeAccentCommand
                       ?? (_changeAccentCommand = new RelayCommand<object>(
                           ChangeAccent));

        #endregion

        #region NewFileCommand

        private RelayCommand _newFileCommand;

        /// <summary>
        ///     Gets the NewFileCommand.
        /// </summary>
        public RelayCommand NewFileCommand => _newFileCommand
                       ?? (_newFileCommand = new RelayCommand(
                           AddNewFile));

        #endregion

        #region ShowFindReplaceCommand

        private RelayCommand _showFileReplaceCommand;

        /// <summary>
        ///     Gets the ShowFindReplaceCommand.
        /// </summary>
        public RelayCommand ShowFindReplaceCommand => _showFileReplaceCommand
                       ?? (_showFileReplaceCommand = new RelayCommand(ShowFindReplace));

        #endregion

        #region ShowSettingsCommand

        private RelayCommand _showSettingsCommand;

        /// <summary>
        ///     Gets the ShowSettingsCommand.
        /// </summary>
        public RelayCommand ShowSettingsCommand => _showSettingsCommand
                       ?? (_showSettingsCommand = new RelayCommand(ExecuteShowSettings));

        #endregion

        #region ChangeViewAsCommand

        private RelayCommand<string> _changeViewAsCommand;

        /// <summary>
        ///     Gets the ChangeViewAsCommand.
        /// </summary>
        public RelayCommand<string> ChangeViewAsCommand => _changeViewAsCommand ?? (_changeViewAsCommand = new RelayCommand<string>(ChangeViewAs));

        #endregion

        #region ExitCommand

        private RelayCommand _exitCommand;

        /// <summary>
        ///     Gets the ExitCommand.
        /// </summary>
        public RelayCommand ExitCommand => _exitCommand
                       ?? (_exitCommand = new RelayCommand(Exit));

        #endregion

        #region ImportCommand

        private RelayCommand<object> _importCommand;

        /// <summary>
        ///     Gets the ImportCommand.
        /// </summary>
        public RelayCommand<object> ImportCommand => _importCommand ?? (_importCommand = new RelayCommand<object>(p => ImportRobot(), CanImport));

        public bool CanImport(object p) => !((p is LanguageBase) | p is Fanuc | p is Kawasaki | p == null);

        #endregion

        #region AddToolCommand

        private RelayCommand<object> _addToolCommand;

        /// <summary>
        ///     Gets the AddToolCommand.
        /// </summary>
        public RelayCommand<object> AddToolCommand => _addToolCommand ?? (_addToolCommand = new RelayCommand<object>(AddTool));

        #endregion

        #region ShowAboutCommand

        private RelayCommand _showAboutCommand;

        /// <summary>
        ///     Gets the ShowAboutCommand.
        /// </summary>
        public RelayCommand ShowAboutCommand => _showAboutCommand ?? (_showAboutCommand = new RelayCommand(ShowAbout));

        #endregion

        #region OpenFileCommand

        private RelayCommand<object> _openFileCommand;

        /// <summary>
        ///     Gets the OpenFileCommand.
        /// </summary>
        public RelayCommand<object> OpenFileCommand => _openFileCommand
                       ?? (_openFileCommand = new RelayCommand<object>(OnOpen));

        #endregion

        #endregion

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

        private void ExecuteShowIO() => ShowIO = !ShowIO;

        private void ShowFindReplace()
        {
            // FindandReplaceControl findandReplaceControl = new FindandReplaceControl(MainWindow.Instance);
            //findandReplaceControl.ShowDialog().GetValueOrDefault();
        }

        private void ExecuteShowSettings() => ShowSettings = !ShowSettings;

        private void ChangeAccent(object param)
        {
            //TODO Track
           // AccentBrush = ThemeManager.Accents.First(x => x.Name == param.ToString());
         //   ThemeManager.ChangeAppStyle(Application.Current, AccentBrush,
            //    ThemeManager.GetAppTheme(CurrentTheme.ToString()));
        }

        private void ChangeTheme(object param)
        {
            //this.CurrentTheme = ((param.ToString() == "Light") ? Theme.Light : Theme.Dark);
         //   ThemeManager.ChangeAppStyle(Application.Current, AccentBrush,
          //      ThemeManager.GetAppTheme(CurrentTheme.ToString()));
        }

        private void OnOpen(object param)
        {
            var directoryName = Path.GetDirectoryName(ActiveEditor.FilePath);
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Allfiles (*.*)|*.*",
                Multiselect = true,
                FilterIndex = 1,
                InitialDirectory = directoryName
            };
            if (openFileDialog.ShowDialog().GetValueOrDefault())
            {
                Open(openFileDialog.FileName);
            }
        }

        public IEditorDocument Open(string filepath)
        {
            var document = OpenFile(filepath);
            ActiveEditor = document;
            ActiveEditor.IsActive = true;
            return document;
        }

        private IEditorDocument OpenFile(string filepath)
        {
            var document = _files.FirstOrDefault(fm => fm.FileName == filepath);
            IEditorDocument result;
            if (document != null)
            {
                result = document;
            }
            else
            {
                document = AbstractLanguageClass.GetViewModel(filepath);
                if (File.Exists(filepath))
                {
                    //document
                    document.Load(filepath);
                    RecentFileList.Instance.InsertFile(filepath);
                    JumpList.AddToRecentCategory(filepath);
                }
                document.IsActive = true;
                _files.Add(document);
                ActiveEditor = document;
                result = document;
            }
            return result;
        }

        public void OpenFile(IVariable variable)
        {
            var document = Open(variable.Path);
            document.SelectText(variable);
        }

        public void AddNewFile()
        {
            _files.Add(new DocumentViewModel(null));
            ActiveEditor = _files.Last();
        }

        public void LoadFile(IEnumerable<string> args)
        {
            foreach (var arg in args)
                Open(arg); 
        }

        private void ChangeViewAs(object param)
        {

            var abstractLanguageClass = ActiveEditor.FileLanguage;

                var text = param.ToString();

                switch (text)
                {
                    case "ABB":
                        var abb = new ABB(ActiveEditor.FilePath);
                        ActiveEditor.FileLanguage = abb;
                        break;
                    case "Fanuc":
                        var fanuc = new Fanuc(ActiveEditor.FilePath);
                        ActiveEditor.FileLanguage = fanuc;
                        break;
                    case "KUKA":
                        var kuka = new KUKA(ActiveEditor.FilePath);
                        ActiveEditor.FileLanguage = kuka;
                        break;
                    case "Kawasaki":
                        var kawasaki = new Kawasaki(ActiveEditor.FilePath);
                        ActiveEditor.FileLanguage = kawasaki;
                        break;
                }
            
        }

        private void Exit() => MainWindow.Instance.Close();

        internal void Close(IEditorDocument fileToClose)
        {
            _files.Remove(fileToClose);
// ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(nameof(ActiveEditor));
        }

        public void AddTool(ToolViewModel toolModel)
        {
            var layoutAnchorable = new LayoutAnchorable();
            if (toolModel != null)
            {
                layoutAnchorable.Title = toolModel.Title;
                layoutAnchorable.Content = toolModel;
                using (var enumerator = (
                    from t in Tools
                    where t.Title == toolModel.Title
                    select t).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        current.IsActive = true;
                        return;
                    }
                }
                toolModel.IsActive = true;
                _tools.Add(toolModel);
                toolModel.IsActive = true;
// ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(Tools));
            }
        }

        [Localizable(false)]
        private void AddTool(object parameter)
        {
            var text = parameter as string;
            ToolViewModel toolModel = null;
            var layoutAnchorable = new LayoutAnchorable();
            var text2 = text;
            switch (text2)
            {
                case "Angle Converter":
                    toolModel = new AngleConvertorViewModel();
                    layoutAnchorable.AutoHideMinWidth = 219.0;
                    goto IL_1EE;
                case "Functions":
                    toolModel = new FunctionViewModel();
                    layoutAnchorable.AutoHideMinWidth = 300.0;
                    goto IL_1EE;
                case "Explorer":
                    layoutAnchorable.Content = new FileExplorerWindow();
                    goto IL_1EE;
                case "Object Browser":
                    toolModel = new ObjectBrowserViewModel();
                    goto IL_1EE;
                case "Output Window":
                    toolModel = new MessageViewModel();
                    goto IL_1EE;
                case "Notes":
                    toolModel = new NotesViewModel();
                    goto IL_1EE;
                case "ArchiveInfo":
                    toolModel = new ArchiveInfoViewModel();
                    goto IL_1EE;
                case "Rename Positions":
                    layoutAnchorable.Content = new RenamePositionWindow();
                    goto IL_1EE;
                case "Shift":
                    layoutAnchorable.Content = new ShiftWindow();
                    goto IL_1EE;
                case "CleanDat":
                    toolModel = new DatCleanHelper();
                    layoutAnchorable.AutoHideMinWidth = DatCleanHelper.Instance.Width;
                    goto IL_1EE;
            }

            var msg = new ErrorMessage("Not Implemented",
                string.Format("Add Tool Parameter of {0} not Implemented", text), MessageType.Error);
            WeakReferenceMessenger.Default.Send<IMessage>(msg);

            IL_1EE:
            if (toolModel != null)
            {
                layoutAnchorable.Title = toolModel.Title;
                layoutAnchorable.Content = toolModel;
                using (var enumerator = (
                    from t in Tools
                    where t.Title == toolModel.Title
                    select t).GetEnumerator())
                {
                    if (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        current.IsActive = true;
                        return;
                    }
                }
                toolModel.IsActive = true;
                _tools.Add(toolModel);
            }
// ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(nameof(Tools));
        }

        private void ImportRobot()
        {
            BringToFront("Object Browser");
            AddTool("ArchiveInfo");
        }

        public void BringToFront(string windowname)
        {
            foreach (
                var current in
                    MainWindow.Instance.DockManager.Layout.Descendents().OfType<LayoutAnchorable>())
            {
                if (current.Title == windowname)
                {
                    current.IsActive = true;
                }
            }
        }

        public void ShowAbout() => new AboutWindow().ShowDialog();

        public bool UserSelectsFileToOpen(out string filePath)
        {
            var openFileDialog = new OpenFileDialog();
            bool result;
// ReSharper disable once PossibleInvalidOperationException
            if (openFileDialog.ShowDialog().Value)
            {
                filePath = openFileDialog.FileName;
                result = true;
            }
            else
            {
                filePath = null;
                result = false;
            }
            return result;
        }

// ReSharper disable once UnusedMember.Global
        public bool UserSelectsNewFilePath(string oldFilePath, out string newFilePath)
        {
            var saveFileDialog = new SaveFileDialog();
            bool result;
// ReSharper disable once PossibleInvalidOperationException
            if (saveFileDialog.ShowDialog().Value)
            {
                newFilePath = saveFileDialog.FileName;
                result = true;
            }
            else
            {
                newFilePath = string.Empty;
                result = false;
            }
            return result;
        }

        // ReSharper disable UnusedMember.Global
        public void ErrorMessage(string msg) => MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);

        // ReSharper disable  UnusedMember.Local
        // ReSharper disable  UnusedParameter.Local

        private void avalonDockHost_AvalonDockLoaded(object sender, EventArgs e) => throw new NotImplementedException();
    }

    // ReSharper enable UnusedMember.Local
    // ReSharper enable UnusedParameter.Local
}