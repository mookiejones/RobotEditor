using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.AvalonEdit;
using RobotEditor.Controls.TextEditor.Bookmarks;
using RobotEditor.Controls.TextEditor.Brackets;
using RobotEditor.Controls.TextEditor.Folding;
using RobotEditor.Controls.TextEditor.IconBar;
using RobotEditor.Controls.TextEditor.Snippets.CompletionData;
using RobotEditor.Controls.TextEditor.Snippets;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Languages.Data;
using RobotEditor.Languages;
using RobotEditor.Messages;
using RobotEditor.Utilities;
using RobotEditor.ViewModel;
using RobotEditor.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.Win32;
using System.Windows.Media;
using System.Runtime.CompilerServices;

namespace RobotEditor.Controls.TextEditor
{
    public partial class AvalonEditor:ICSharpCode.AvalonEdit.TextEditor
    {

        #region Constants

        #endregion

        #region Dependency Properties

        public new string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region Members

        public IList<ICompletionDataProvider> CompletionDataProviders { get; set; }

        public static readonly DependencyProperty CompletionWindowProperty =
            DependencyProperty.Register("CompletionWindow", typeof(CompletionWindow), typeof(AvalonEditor));

        private readonly MyBracketSearcher _bracketSearcher = new();

        private readonly IconBarManager _iconBarManager;
        private readonly IconBarMargin _iconBarMargin;
        private readonly ReadOnlyObservableCollection<IVariable> _readonlyVariables = null;
        private readonly ObservableCollection<IVariable> _variables = new();
        private BracketHighlightRenderer _bracketRenderer;
        private string _fileSave = string.Empty;
        private string _filename = string.Empty;

        private FoldingManager _foldingManager;
        private object _foldingStrategy;
        private KeyEventArgs _lastKeyUpArgs;
        private ToolTip _toolTip;
        private static BitmapImage _imgMethod;
        // ReSharper disable once UnassignedField.Compiler
        private static readonly BitmapImage _imgStruct = ImageHelper.LoadBitmap(Global.ImgStruct);
        private static BitmapImage _imgEnum;
        private static BitmapImage _imgSignal;
        // ReSharper disable once UnassignedField.Compiler
        private static readonly BitmapImage _imgField;
        private static BitmapImage _imgXyz;

        private AbstractLanguageClass _filelanguage;

        #endregion

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
            typeof(AvalonEditor), new PropertyMetadata((obj, args) =>
            {
                if(obj is AvalonEditor target)
                {
                    target.Text = (string)args.NewValue;
                }

            }));

        private void ChangeCommandBindings()
        {
            ICollection<CommandBinding> commandBindings = base.TextArea.DefaultInputHandler.Editing.CommandBindings;
            foreach (CommandBinding current in commandBindings)
            {
                if (current.Command == AvalonEditCommands.DeleteLine)
                {
                    RoutedCommand command = new("DeleteLine", typeof(AvalonEditor), new InputGestureCollection
            {
                new KeyGesture(Key.L, ModifierKeys.Control)
            });
                    current.Command = command;
                    break;
                }
            }
        }

        public new EditorOptions Options { get => EditorOptions.Instance; set
            {
                EditorOptions.Instance = value;
                base.Options = value;
                OnPropertyChanged(nameof(Options));
            }
        }

        public AvalonEditor()
        {
            try
            {
                _toolTip = new ToolTip();
                ChangeCommandBindings();
                CreateKeyBindings();
              
                CompletionDataProviders = new List<ICompletionDataProvider>()
                {
                    new SnippetCompletionDataProvider()
                };
                _iconBarMargin = new IconBarMargin(_iconBarManager = new IconBarManager());
                InitializeMyControl();
                MouseHoverStopped += (s, e) => _toolTip.IsOpen = false;
                MouseHover += Mouse_OnHover;
                PreviewMouseWheel += EditorPreviewMouseWheel;
                GotFocus += TextEditorGotFocus;
                PreviewKeyDown += TextEditor_PreviewKeyDown;
            }
            finally
            {
                InvokeModifiedChanged(false);
            }
        }

        private void CreateKeyBindings()
        {
            InputBindings.Add(new KeyBinding(GotoCommand, new KeyGesture(Key.G, ModifierKeys.Control)));
            InputBindings.Add(new KeyBinding(ApplicationCommands.Find, new KeyGesture(Key.F, ModifierKeys.Control)));
            InputBindings.Add(new KeyBinding(ReplaceCommand, new KeyGesture(Key.R, ModifierKeys.Control)));
            InputBindings.Add(new KeyBinding(ReloadCommand, new KeyGesture(Key.R,ModifierKeys.Alt)));
            InputBindings.Add(new KeyBinding(AddTimeStampCommand, new KeyGesture(Key.D, ModifierKeys.Control)));
        }
        public event EventHandler IsModifiedChanged;
        public void InvokeModifiedChanged(bool isNowModified)
        {
            IsModified = isNowModified;
            IsModifiedChanged?.Invoke(this, new EventArgs());

        }

        private CompletionWindow CompletionWindow
        {
            get => (CompletionWindow)GetValue(CompletionWindowProperty);
            set => SetValue(CompletionWindowProperty, value);
        }
        public  string GetWordBeforeCaret()
        {
          
            int offset = TextArea.Caret.Offset;
            int num = Document.FindPrevWordStart(offset);
            return num < 0 ? string.Empty : Document.GetText(num, offset - num);
        }
        public IList<ICompletionData> CompletionData
        {
            get; // ReSharper disable once UnusedAutoPropertyAccessor.Local
            private set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties

        public int Line => TextArea.Caret.Column;

        public int Column => TextArea.Caret.Column;

        public int Offset => TextArea.Caret.Offset;

        public string FileSave
        {
            get => _fileSave;
            set
            {
                _fileSave = value;
                OnPropertyChanged("FileSave");
            }
        }

        #region EditorType

        private EDITORTYPE _editortype;

        public EDITORTYPE EditorType
        {
            get => _editortype;
            set
            {
                _editortype = value;
                OnPropertyChanged("EditorType");
            }
        }

        #endregion

        #region SelectedVariable

        private IVariable _selectedVariable;

        public IVariable SelectedVariable
        {
            get => _selectedVariable;
            set
            {
                _selectedVariable = value;
                SelectText(_selectedVariable);
                OnPropertyChanged("SelectedVariable");
            }
        }

        #endregion

        #region Filename

        public string Filename
        {
            get => _filename;
            set
            {
                _filename = value;
                OnPropertyChanged("Filename");
                OnPropertyChanged("Title");
            }
        }

        #endregion

        #region FileLanguage

        public AbstractLanguageClass FileLanguage
        {
            get => _filelanguage;
            set
            {
                _filelanguage = value;
                OnPropertyChanged("FileLanguage");
            }
        }

        #endregion

        #region Variables

        public ReadOnlyObservableCollection<IVariable> Variables => _readonlyVariables ?? new ReadOnlyObservableCollection<IVariable>(_variables);

        #endregion

        #endregion

        #region Commands

        #region VariableDoubleClickCommand

        private RelayCommand<object> _variableDoubleClickCommand;

        /// <summary>
        ///     Gets the VariableDoubleClickCommand.
        /// </summary>
        public RelayCommand<object> VariableDoubleClickCommand => _variableDoubleClickCommand ?? (_variableDoubleClickCommand = new RelayCommand<object>(
                    ExecuteVariableDoubleClickCommand,
                    CanExecuteVariableDoubleClickCommand));

        private void ExecuteVariableDoubleClickCommand(object parameter) => SelectText((IVariable)parameter);

        private bool CanExecuteVariableDoubleClickCommand(object parameter) => true;

        #endregion

        #region UndoCommand

        private RelayCommand _undoCommand;

        /// <summary>
        ///     Gets the UndoCommand.
        /// </summary>
        public RelayCommand UndoCommand => _undoCommand ?? (_undoCommand = new RelayCommand(
                    ExecuteUndoCommand,
                    CanExecuteUndoCommand));

        private void ExecuteUndoCommand() => _ = Undo();

        private bool CanExecuteUndoCommand() => true; //CanUndo();

        #endregion

        #region RedoCommand

        private RelayCommand<object> _redoCommand;

        /// <summary>
        ///     Gets the RedoCommand.
        /// </summary>
        public RelayCommand<object> RedoCommand => _redoCommand ?? (_redoCommand = new RelayCommand<object>(
                    ExecuteRedoCommand,
                    CanExecuteRedoCommand));

        private void ExecuteRedoCommand(object parameter) => _ = Redo();

        private bool CanExecuteRedoCommand(object parameter) => true; //CanRedo();

        #endregion

        #region SaveCommand

        private RelayCommand _saveCommand;

        /// <summary>
        ///     Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand => _saveCommand
                       ?? (_saveCommand = new RelayCommand(Save, CanSave));

        #endregion

        #region SaveAsCommand

        private RelayCommand _saveAsCommand;

        /// <summary>
        ///     Gets the SaveAsCommand.
        /// </summary>
        public RelayCommand SaveAsCommand => _saveAsCommand
                       ?? (_saveAsCommand = new RelayCommand(SaveAs, CanSave));

        #endregion

        #region ReplaceCommand

        private RelayCommand _replaceCommand;

        /// <summary>
        ///     Gets the ReplaceCommand.
        /// </summary>
        public RelayCommand ReplaceCommand => _replaceCommand
                       ?? (_replaceCommand = new RelayCommand(Replace));

        #endregion

        #region GotoCommand

        private RelayCommand _gotoCommand;

        /// <summary>
        ///     Gets the GotoCommand.
        /// </summary>
        public RelayCommand GotoCommand => _gotoCommand
                       ?? (_gotoCommand = new RelayCommand(Goto, CanGoto));


        public bool CanGoto() => !string.IsNullOrEmpty(Text);

        #endregion

        #region OpenAllFoldsCommand

        private RelayCommand _openAllFoldsCommand;

        /// <summary>
        ///     Gets the OpenAllFoldsCommand.
        /// </summary>
        public RelayCommand OpenAllFoldsCommand => _openAllFoldsCommand
                       ?? (_openAllFoldsCommand = new RelayCommand(ExecuteOpenAllFoldsCommand, CanChangeFoldStatus));

        private void ExecuteOpenAllFoldsCommand() => ChangeFoldStatus(true);

        private bool CanChangeFoldStatus() => _foldingManager != null && _foldingManager.AllFoldings.Any();

        #endregion

        #region ToggleCommentCommand

        private RelayCommand _toggleCommentCommand;

        /// <summary>
        ///     Gets the ToggleCommentCommand.
        /// </summary>
        public RelayCommand ToggleCommentCommand => _toggleCommentCommand
                       ?? (_toggleCommentCommand = new RelayCommand(ToggleComment, CanToggleComment));

        public bool CanToggleComment() => !string.IsNullOrEmpty(FileLanguage.CommentChar);

        #endregion

        #region ToggleFoldsCommand

        private RelayCommand _toggleFoldsCommand;

        /// <summary>
        ///     Gets the ToggleFoldsCommand.
        /// </summary>
        public RelayCommand ToggleFoldsCommand => _toggleFoldsCommand
                       ?? (_toggleFoldsCommand = new RelayCommand(ToggleFolds, CanToggleFolds));

        public bool CanToggleFolds() => _foldingManager != null && _foldingManager.AllFoldings.Any();

        #endregion

        #region ToggleAllFoldsCommand

        private RelayCommand _toggleAllFoldsCommand;

        /// <summary>
        ///     Gets the ToggleAllFoldsCommand.
        /// </summary>
        public RelayCommand ToggleAllFoldsCommand => _toggleAllFoldsCommand
                       ?? (_toggleAllFoldsCommand = new RelayCommand(ToggleAllFolds, CanToggleFolds));

        #endregion

        #region CloseAllFoldsCommand

        private RelayCommand<object> _closeAllFoldsCommand;

        /// <summary>
        ///     Gets the CloseAllFoldsCommand.
        /// </summary>
        public RelayCommand<object> CloseAllFoldsCommand => _closeAllFoldsCommand ?? (_closeAllFoldsCommand = new RelayCommand<object>(
                    ExecuteCloseAllFoldsCommand,
                    CanExecuteCloseAllFoldsCommand));

        private void ExecuteCloseAllFoldsCommand(object parameter) => ChangeFoldStatus(true);

        private bool CanExecuteCloseAllFoldsCommand(object parameter) => _foldingManager != null && _foldingManager.AllFoldings.Any();

        #endregion

        #region AddTimeStampCommand

        private RelayCommand _addTimeStampCommand;

        /// <summary>
        ///     Gets the AddTimeStampCommand.
        /// </summary>
        public RelayCommand AddTimeStampCommand => _addTimeStampCommand ?? (_addTimeStampCommand = new RelayCommand(
                    ExecuteAddTimeStampCommand));

        private void ExecuteAddTimeStampCommand() => AddTimeStamp(true);

        #endregion

        #region FindCommand

        private RelayCommand _findCommand;

        /// <summary>
        ///     Gets the FindCommand.
        /// </summary>
        public RelayCommand FindCommand => _findCommand
                       ?? (_findCommand = new RelayCommand(ExecuteFindCommand, CanFind));

        private void ExecuteFindCommand() => ChangeFoldStatus(true);

        public bool CanFind() => _foldingManager != null && _foldingManager.AllFoldings.Any();

        #endregion

        #region ReloadCommand

        private RelayCommand _reloadCommand;

        /// <summary>
        ///     Gets the ReloadCommand.
        /// </summary>
        public RelayCommand ReloadCommand => _reloadCommand
                       ?? (_reloadCommand = new RelayCommand(Reload));

        #endregion

        #region ShowDefinitionsCommand

        private RelayCommand _showDefinitionsCommand;

        /// <summary>
        ///     Gets the ShowDefinitionsCommand.
        /// </summary>
        public RelayCommand ShowDefinitionsCommand => _showDefinitionsCommand
                       ?? (_showDefinitionsCommand = new RelayCommand(ShowDefinitions, CanShowDefinitions));

        public bool CanShowDefinitions() => _foldingManager != null;

        #endregion

        #region CutCommand

        private RelayCommand _cutCommand;

        /// <summary>
        ///     Gets the CutCommand.
        /// </summary>
        public RelayCommand CutCommand => _cutCommand
                       ?? (_cutCommand = new RelayCommand(ExecuteCutCommand, CanCut));

        private void ExecuteCutCommand() => Cut();

        public bool CanCut() => Text.Length > 0;

        #endregion

        #region CopyCommand

        private RelayCommand _copyCommand;

        /// <summary>
        ///     Gets the CopyCommand.
        /// </summary>
        public RelayCommand CopyCommand => _copyCommand
                       ?? (_copyCommand = new RelayCommand(ExecuteCopy, CanCopy));


        public void ExecuteCopy() => Copy();

        public bool CanCopy() => Text.Length > 0;

        #endregion

        #region FunctionWindowClickCommand

        private RelayCommand<object> _functionWindowClickCommand;

        /// <summary>
        ///     Gets the FunctionWindowClickCommand.
        /// </summary>
        public RelayCommand<object> FunctionWindowClickCommand => _functionWindowClickCommand ??
                       (_functionWindowClickCommand = new RelayCommand<object>(OpenFunctionItem));

        #endregion

        #region ChangeIndentCommand

        private RelayCommand<object> _changeIndentCommand;

        /// <summary>
        ///     Gets the ChangeIndentCommand.
        /// </summary>
        public RelayCommand<object> ChangeIndentCommand => _changeIndentCommand
                       ?? (_changeIndentCommand = new RelayCommand<object>(ChangeIndent));

        #endregion

        #region PasteCommand

        private RelayCommand _pasteCommand;

        /// <summary>
        ///     Gets the PasteCommand.
        /// </summary>
        public RelayCommand PasteCommand => _pasteCommand ?? (_pasteCommand = new RelayCommand(
                    ExecutePasteCommand,
                    CanExecutePasteCommand));

        private void ExecutePasteCommand() => Paste();

        private bool CanExecutePasteCommand() => Clipboard.ContainsText();

        #endregion

        #endregion

        /// <summary>
        ///     Add Time Stamp to TextBox
        /// </summary>
        /// <param name="b"></param>
        // ReSharper disable UnusedParameter.Local
        private void AddTimeStamp(bool b)
        // ReSharper restore UnusedParameter.Local
        {
            SnippetTextElement item = new()
            {
                Text = "\r\n; * "
            };
            SnippetTextElement item2 = new()
            {
                Text = "By : "
            };
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            if (current != null)
            {
                SnippetReplaceableTextElement item3 = new()
                {
                    Text = current.Name
                };
                SnippetTextElement item4 = new()
                {
                    Text = DateTime.Now.ToString(((EditorOptions)Options).TimestampFormat)
                };
                Snippet snippet = new()
                {
                    Elements =
                    {
                        item,
                        item2,
                        item3,
                        item,
                        item4,
                        item
                    }
                };
                snippet.Insert(TextArea);
            }
        }


        private void OpenFunctionItem(object parameter)
        {
            IVariable var = (IVariable)((ListViewItem)parameter).Content;
            SelectText(var);
        }

        private void InitializeMyControl()
        {
            TextArea.LeftMargins.Insert(0, _iconBarMargin);
            _ = SearchPanel.Install(TextArea);
            TextArea.TextEntered += TextEntered;
            TextArea.Caret.PositionChanged += CaretPositionChanged;
            DataContext = this;
        }

        /// <summary>
        ///     HighlightBrackets
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
// ReSharper disable UnusedParameter.Local
        private void HighlightBrackets(object sender, EventArgs e)
        {
            BracketSearchResult highlight = _bracketSearcher.SearchBracket(Document, TextArea.Caret.Offset);
            _bracketRenderer.SetHighlight(highlight);
        }

        // ReSharper restore UnusedParameter.Local

        private void CaretPositionChanged(object sender, EventArgs e)
        {
            UpdateLineTransformers();
            if (sender is Caret)
            {
                OnPropertyChanged("Line");
                OnPropertyChanged("Column");
                OnPropertyChanged("Offset");
                FileSave = (!string.IsNullOrEmpty(Filename))
                    ? File.GetLastWriteTime(Filename).ToString(CultureInfo.InvariantCulture)
                    : string.Empty;
            }
            HighlightBrackets(sender, e);
        }

        private void UpdateLineTransformers()
        {
            TextArea.TextView.BackgroundRenderers.Clear();
            if (Options is EditorOptions editorOptions && editorOptions.HighlightCurrentLine)
            {
                TextArea.TextView.BackgroundRenderers.Add(new BackgroundRenderer(Document.GetLineByOffset(CaretOffset)));
            }
            if (_bracketRenderer == null)
            {
                _bracketRenderer = new BracketHighlightRenderer(TextArea.TextView);
            }
            else
            {
                TextArea.TextView.BackgroundRenderers.Add(_bracketRenderer);
            }
        }

        /// <summary>
        ///     Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyUp" /> attached event reaches an element in
        ///     its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (_lastKeyUpArgs == null)
            {
                _lastKeyUpArgs = e;
            }
            else
            {
                ModifierKeys modifiers = Keyboard.Modifiers;
                if (modifiers == ModifierKeys.Control)
                {
                    Key key = e.Key;
                    if (key == Key.O)
                    {
                        if (!string.IsNullOrEmpty(FileLanguage.CommentChar))
                        {
                            ToggleFolds();
                        }
                    }
                }
                _lastKeyUpArgs = e;
                base.OnKeyUp(e);
            }
        }

        private void AddBookMark(int lineNumber, BitmapImage img)
        {
            BookmarkImage image = new(img);
            _iconBarManager.Bookmarks.Add(new ClassMemberBookmark(lineNumber, image));
        }


        private void FindMatches(Regex matchstring, BitmapImage img)
        {
            if (!string.IsNullOrEmpty(matchstring.ToString()))
            {
                // Was To lower invariant
                Match match = matchstring.Match(Text);

                while (match.Success)
                {
                    _variables.Add(new Variable
                    {
                        Declaration = match.Groups[0].ToString(),
                        Offset = match.Index,
                        Type = match.Groups[1].ToString(),
                        Name = match.Groups[2].ToString(),
                        Value = match.Groups[3].ToString(),
                        Path = Filename,
                        Icon = img
                    });
                    DocumentLine lineByOffset = Document.GetLineByOffset(match.Index);
                    AddBookMark(lineByOffset.LineNumber, img);
                    match = match.NextMatch();
                }
                if (FileLanguage is KUKA)
                {
                    match =
                        matchstring.Match((string.CompareOrdinal(Text, FileLanguage.SourceText) == 0)
                            ? FileLanguage.DataText
                            : FileLanguage.SourceText);
                    while (match.Success)
                    {
                        _variables.Add(new Variable
                        {
                            Declaration = match.Groups[0].ToString(),
                            Offset = match.Index,
                            Type = match.Groups[1].ToString(),
                            Name = match.Groups[2].ToString(),
                            Value = match.Groups[3].ToString(),
                            Path = Filename,
                            Icon = img
                        });
                        match = match.NextMatch();
                    }
                }
            }
        }


        private void CreateImages()
        {
            if (_imgMethod == null)
            {
                _imgMethod = ImageHelper.LoadBitmap(Global.ImgMethod);
            }

            if (_imgStruct == null)
            {
                _imgMethod = ImageHelper.LoadBitmap(Global.ImgStruct);
            }

            if (_imgEnum == null)
            {
                _imgEnum = ImageHelper.LoadBitmap(Global.ImgEnum);
            }

            if (_imgSignal == null)
            {
                _imgSignal = ImageHelper.LoadBitmap(Global.ImgSignal);
            }

            if (_imgXyz == null)
            {
                _imgXyz = ImageHelper.LoadBitmap(Global.ImgXyz);
            }
        }

        private void FindBookmarkMembers()
        {
            CreateImages();

            if (FileLanguage != null)
            {
                _iconBarManager.Bookmarks.Clear();
                _variables.Clear();
                FindMatches(FileLanguage.MethodRegex, _imgMethod);
                FindMatches(FileLanguage.StructRegex, _imgStruct);
                FindMatches(FileLanguage.FieldRegex, _imgField);
                FindMatches(FileLanguage.SignalRegex, _imgSignal);
                FindMatches(FileLanguage.EnumRegex, _imgEnum);
                FindMatches(FileLanguage.XYZRegex, _imgXyz);
            }
        }


        /// <inheritdoc cref="M:System.Windows.IWeakEventListener.ReceiveWeakEvent(System.Type,System.Object,System.EventArgs)" />
        /// //TODO Reimplement this
        protected override bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            switch (managerType.Name)
            {
                case "TextChanged":
                    FindBookmarkMembers();
                    //IsModified = true;
                    UpdateFolds();
                    break;
            }
            return base.ReceiveWeakEvent(managerType, sender, e);
        }

        private void ChangeIndent(object param)
        {
            try
            {
                bool flag = Convert.ToBoolean(param);
                DocumentLine lineByOffset = Document.GetLineByOffset(SelectionStart);
                DocumentLine lineByOffset2 = Document.GetLineByOffset(SelectionStart + SelectionLength);
                int num = 0;
                using (Document.RunUpdate())
                {
                    DocumentLine documentLine = lineByOffset;
                    while (documentLine.LineNumber < lineByOffset2.LineNumber + 1)
                    {
                        string line = GetLine(documentLine.LineNumber);
                        Regex regex = new("(^[\\s]+)");
                        Match match = regex.Match(line);
                        if (match.Success)
                        {
                            num = match.Groups[1].Length;
                        }
                        if (flag)
                        {
                            Document.Insert(documentLine.Offset + num, " ");
                        }
                        else
                        {
                            num = (num > 1) ? (num - 1) : num;
                            if (num >= 1)
                            {
                                Document.Replace(documentLine.Offset, line.Length, line.Substring(1));
                            }
                        }
                        documentLine = documentLine.NextLine;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage msg = new("Editor.ChangeIndent", ex);
                _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
            }
        }

        private void ToggleComment()
        {
            if (FileLanguage != null)
            {
                DocumentLine lineByOffset = Document.GetLineByOffset(SelectionStart);
                DocumentLine lineByOffset2 = Document.GetLineByOffset(SelectionStart + SelectionLength);
                using (Document.RunUpdate())
                {
                    DocumentLine documentLine = lineByOffset;
                    while (documentLine.LineNumber < lineByOffset2.LineNumber + 1)
                    {
                        string line = GetLine(documentLine.LineNumber);
                        if (FileLanguage.IsLineCommented(line))
                        {
                            Document.Insert(FileLanguage.CommentOffset(line) + documentLine.Offset,
                                FileLanguage.CommentChar);
                        }
                        else
                        {
                            string text = FileLanguage.CommentReplaceString(line);
                            Document.Replace(documentLine.Offset, line.Length, text);
                        }
                        documentLine = documentLine.NextLine;
                    }
                }
            }
        }

        private bool CanSave() => File.Exists(Filename) ? IsModified : IsModified;

        private void Replace()
        {
            FindAndReplaceWindow.Instance.Left = Mouse.GetPosition(this).X;
            FindAndReplaceWindow.Instance.Top = Mouse.GetPosition(this).Y;
        }

        private void Goto()
        {
            GotoViewModel dataContext = new(this);
            GotoWindow gotoDialog = new()
            {
                DataContext = dataContext
            };
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            _ = gotoDialog.ShowDialog().GetValueOrDefault();
        }

        private bool IsFileLocked(System.IO.FileInfo file)
        {
            FileStream fileStream = null;
            if (!File.Exists(file.FullName))
            {
            }
            else
            {
                try
                {
                    fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (IOException ex)
                {
                    ErrorMessage msg = new("File is locked!", ex);
                    _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
                    return true;
                }
                finally
                {
                    fileStream?.Close();
                }
            }
            return false;
        }

        private string GetFilename()
        {
            SaveFileDialog ofd = new() { Title = "Save As", Filter = "All _files(*.*)|*.*" };

            if (!string.IsNullOrEmpty(Filename))
            {
                ofd.FileName = Filename;
                ofd.Filter += string.Format("|Current Type (*{0})|*{0}", Path.GetExtension(Filename));
                ofd.FilterIndex = 2;
                ofd.DefaultExt = Path.GetExtension(Filename);
            }

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            bool result = ofd.ShowDialog().GetValueOrDefault();
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            return result ? ofd.FileName : string.Empty;
        }

        public void SaveAs()
        {
            string filename = GetFilename();
            if (!string.IsNullOrEmpty(filename))
            {
                Filename = filename;
                bool flag = IsFileLocked(new System.IO.FileInfo(Filename));
                if (!flag)
                {
                    File.WriteAllText(Filename, Text);
                    RaisePropertyChanged("Title");

                    OutputWindowMessage msg = new()
                    {
                        Title = "File Saved",
                        Description = Filename,
                    };

                    _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
                }
            }
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(Filename))
            {
                string filename = GetFilename();
                if (string.IsNullOrEmpty(filename))
                {
                    return;
                }
                Filename = filename;
            }
            if (!IsFileLocked(new System.IO.FileInfo(Filename)))
            {
                File.WriteAllText(Filename, Text);
                FileSave = File.GetLastWriteTime(Filename).ToString(CultureInfo.InvariantCulture);
                IsModified = false;
            }
        }

        /// <summary>
        ///     Raises the <see cref="E:ICSharpCode.AvalonEdit.TextEditor.OptionChanged" /> event.
        /// </summary>
        protected override void OnOptionChanged(PropertyChangedEventArgs e)
        {
            base.OnOptionChanged(e);
            Console.WriteLine(e.PropertyName);
            string propertyName = e.PropertyName;
            if (propertyName != null)
            {
                if (propertyName == "EnableFolding")
                {
                    UpdateFolds();
                }
            }
        }

        public void SetHighlighting()
        {
            try
            {
                if (Filename != null)
                {
                    SyntaxHighlighting =
                        HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(Filename));
                }
            }
            catch (Exception ex)
            {
                ErrorMessage msg = new(string.Format("Could not load Syntax Highlighting for {0}", Filename), ex,
                    MessageType.Error);
                _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
            }
        }

        private void Complete(char newChar)
        {

        }
        public string DocumentType { get; set; }
        public bool UseCodeCompletion { get; set; }
        private void TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (!base.IsReadOnly && e.Text.Length == 1)
            {
                char newChar = e.Text[0];
                if (UseCodeCompletion)
                {
                    Complete(newChar);
                }
            }
            if (CompletionWindow != null)
            {
                return;
            }

            string wordBeforeCaret = this.GetWordBeforeCaret(GetWordParts());

            if (SnippetManager.HasSnippetsFor(wordBeforeCaret, DocumentType))
            {
                insightWindow = new InsightWindow(base.TextArea)
                {
                    Content = "Press tab to enter snippet",
                    Background = Brushes.Linen
                };
                insightWindow.Show();
                return;
            }
            if (FileLanguage != null && !(FileLanguage is LanguageBase))
            {
                string text = FindWord();
                if (IsModified || IsModified)
                {
                    UpdateFolds();
                }
                if (text == null || !(string.IsNullOrEmpty(text) | text.Length < 3))
                {
                    ShowCompletionWindow(text);
                }
            }
        }


        private InsightWindow insightWindow;
        private void ShowCompletionWindow(string currentword)
        {
            CompletionWindow = new CompletionWindow(TextArea);
            IEnumerable<ICompletionData> completionItems = GetCompletionItems();
            foreach (ICompletionData current in completionItems)
            {
                CompletionWindow.CompletionList.CompletionData.Add(current);
            }
            CompletionWindow.Closed += delegate { CompletionWindow = null; };
            CompletionWindow.CloseWhenCaretAtBeginning = true;
            CompletionWindow.CompletionList.SelectItem(currentword);
            if (CompletionWindow.CompletionList.SelectedItem != null)
            {
                CompletionWindow.Show();
            }
        }

        private IEnumerable<ICompletionData> GetCompletionItems()
        {
            List<ICompletionData> list = new();
            list.AddRange(HighlightList());
            list.AddRange(ObjectBrowserCompletionList());
            return list.ToArray();
        }

        private IEnumerable<ICompletionData> HighlightList()
        {
            List<CodeCompletion> items = new();
            foreach (CodeCompletion current in
                from rule in SyntaxHighlighting.MainRuleSet.Rules
                select rule.Regex.ToString()
                into parseString
                let start = parseString.IndexOf(">", StringComparison.Ordinal) + 1
                let end = parseString.LastIndexOf(")", StringComparison.Ordinal)
                select parseString.Substring(start, end - start)
                into parseString1
                select parseString1.Split(new[]
                {
                    '|'
                })
                into spl
                from item in
                from t in spl
                where !string.IsNullOrEmpty(t)
                select new CodeCompletion(t.Replace("\\b", ""))
                    into item
                where !items.Contains(item) && char.IsLetter(item.Text, 0)
                select item
                select item)
            {
                items.Add(current);
            }
            return items.ToArray();
        }

        private IEnumerable<ICompletionData> ObjectBrowserCompletionList() => (
                from v in FileLanguage.Fields
                where v.Type != "def" && v.Type != "deffct"
                select new CodeCompletion(v.Name)
                {
                    Image = v.Icon
                }).ToArray<ICompletionData>();

        private void TextEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (CompletionWindow != null)
            {
                if (e.Key == Key.Tab)
                {
                    CompletionWindow.CompletionList.RequestInsertion(e);
                }
                if (e.Key == Key.Return)
                {
                    CompletionWindow = null;
                }
            }
        }

        public void ReplaceAll()
        {
            Regex regexPattern = FindReplaceViewModel.Instance.RegexPattern;
            Match match = regexPattern.Match(Text);
            while (match.Success)
            {
                _ = Document.GetLineByOffset(match.Index);
                _ = regexPattern.Replace(FindReplaceViewModel.Instance.LookFor, FindReplaceViewModel.Instance.ReplaceWith,
                    match.Index);
                match = match.NextMatch();
            }
        }

        public void ReplaceText()
        {
            FindText();
            SelectedText = SelectedText.Replace(SelectedText, FindReplaceViewModel.Instance.ReplaceWith);
        }

        public void FindText()
        {
            int num = Text.IndexOf(FindReplaceViewModel.Instance.LookFor, CaretOffset, StringComparison.Ordinal);
            if (num > -1)
            {
                _ = Document.GetLineByOffset(num);
                JumpTo(new Variable
                {
                    Offset = num
                });
                SelectionStart = num;
                SelectionLength = FindReplaceViewModel.Instance.LookFor.Length;
            }
            else
            {
                FindReplaceViewModel.Instance.SearchResult = "No Results Found, Starting Search from Beginning";
                CaretOffset = 0;
            }
        }

        private void JumpTo(IVariable i)
        {
            TextLocation location = Document.GetLocation(Convert.ToInt32(i.Offset));
            ScrollTo(location.Line, location.Column);
            SelectionStart = Convert.ToInt32(i.Offset);
            SelectionLength = i.Value.Length;
            _ = Focus();
            if (EditorOptions.Instance.EnableAnimations)
            {
                _ = Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(DisplayCaretHighlightAnimation));
            }
        }

        private void DisplayCaretHighlightAnimation()
        {
            if (TextArea != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(TextArea.TextView);
                if (adornerLayer != null)
                {
                    CaretHighlightAdorner adorner = new(TextArea);
                    adornerLayer.Add(adorner);
                }
            }
        }

        public void SelectText(IVariable var)
        {
            if (var != null)
            {
                if (var.Name == null)
                {
                    throw new ArgumentNullException("var");
                }
                DocumentLine lineByOffset = Document.GetLineByOffset(var.Offset);
                TextArea.Caret.BringCaretToView();
                CaretOffset = lineByOffset.Offset;
                ScrollToLine(lineByOffset.LineNumber);
                ReadOnlyCollection<FoldingSection> foldingsAt = _foldingManager.GetFoldingsAt(lineByOffset.Offset);
                if (foldingsAt.Count > 0)
                {
                    FoldingSection foldingSection = foldingsAt[0];
                    foldingSection.IsFolded = false;
                }
                FindText(var.Offset, var.Name);
                JumpTo(var);
            }
        }

        private void FindText(int startOffset, string text)
        {
            int selectionStart = Text.IndexOf(text, startOffset, StringComparison.OrdinalIgnoreCase);
            SelectionStart = selectionStart;
            SelectionLength = text.Length;
        }

        public void FindText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            SelectionStart = Text.IndexOf(text, CaretOffset, StringComparison.Ordinal);
        }

        public void ShowFindDialog() => _ = FindAndReplaceWindow.Instance.ShowDialog();

        private void EditorPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!(Options is EditorOptions editorOptions) || editorOptions.MouseWheelZoom)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    if (e.Delta <= 0 || FontSize >= 50.0)
                    {
                        FontSize -= 1.0;
                    }
                    else
                    {
                        if (e.Delta > 0 && FontSize > 10.0)
                        {
                            FontSize += 1.0;
                        }
                    }
                    e.Handled = true;
                }
            }
        }

        [Localizable(false)]
        private void UpdateFolds()
        {
            bool flag = Options is EditorOptions editorOptions && editorOptions.EnableFolding;
            if (SyntaxHighlighting == null)
            {
                _foldingStrategy = null;
            }
            if (File.Exists(Filename))
            {
                if (Path.GetExtension(Filename) == ".xml" || Path.GetExtension(Filename) == ".cfg")
                {
                    _foldingStrategy = new XmlFoldingStrategy();
                }
                else
                {
                    if (FileLanguage != null)
                    {
                        _foldingStrategy = FileLanguage.FoldingStrategy;
                    }
                }
                if (_foldingStrategy != null && flag)
                {
                    if (_foldingManager == null)
                    {
                        _foldingManager = FoldingManager.Install(TextArea);
                    }

                    if (_foldingStrategy is XmlFoldingStrategy xmlStrategy)
                    {
                        xmlStrategy.UpdateFoldings(_foldingManager, Document);
                    }
                    else
                    {
                        ((AbstractFoldingStrategy)_foldingStrategy).UpdateFoldings(_foldingManager, Document);

                    }

                    RegisterFoldTitles();
                }
                else
                {
                    if (_foldingManager != null)
                    {
                        FoldingManager.Uninstall(_foldingManager);
                        _foldingManager = null;
                    }
                }
            }
        }

        private void RegisterFoldTitles()
        {
            if (!(DocumentViewModel.Instance.FileLanguage is LanguageBase) || !(Path.GetExtension(Filename) == ".xml"))
            {
                foreach (FoldingSection current in _foldingManager.AllFoldings)
                {
                    current.Title = DocumentViewModel.Instance.FileLanguage.FoldTitle(current, Document);
                }
            }
        }

        private string GetLine(int idx)
        {
            DocumentLine lineByNumber = Document.GetLineByNumber(idx);
            return Document.GetText(lineByNumber.Offset, lineByNumber.Length);
        }

        public string FindWord()
        {
            string line = GetLine(TextArea.Caret.Line);
            string text = line;
            char[] anyOf = new[]
            {
                ' ',
                '=',
                '(',
                ')',
                '[',
                ']',
                '<',
                '>',
                '\r',
                '\n'
            };
            int num = line.IndexOfAny(anyOf, TextArea.Caret.Column - 1);
            if (num > -1)
            {
                text = line.Substring(0, num);
            }
            int num2 = text.LastIndexOfAny(anyOf) + 1;
            if (num2 > -1)
            {
                text = text.Substring(num2).Trim();
            }
            return text;
        }

        private bool GetCurrentFold(TextViewPosition loc)
        {
            int offset = Document.GetOffset(loc.Location);
            ReadOnlyCollection<FoldingSection> foldingsAt = _foldingManager.GetFoldingsAt(offset);
            bool result;
            if (foldingsAt.Count == 0)
            {
                result = false;
            }
            else
            {
                _toolTip = new ToolTip
                {
                    Style = (Style)FindResource("FoldToolTipStyle"),
                    DataContext = foldingsAt,
                    PlacementTarget = this,
                    IsOpen = true
                };
                result = true;
            }
            return result;
        }

        private void Mouse_OnHover(object sender, MouseEventArgs e)
        {
            if (_foldingManager != null)
            {
                TextViewPosition? positionFromPoint = GetPositionFromPoint(e.GetPosition(this));
                if (positionFromPoint.HasValue)
                {
                    e.Handled = GetCurrentFold(positionFromPoint.Value);
                }
            }
        }

        private void ToggleFolds()
        {
            if (_foldingManager != null)
            {
                FoldingSection foldingSection =
                    _foldingManager.GetNextFolding(TextArea.Document.GetOffset(TextArea.Caret.Line,
                        TextArea.Caret.Column));
                if (foldingSection == null ||
                    Document.GetLineByOffset(foldingSection.StartOffset).LineNumber != TextArea.Caret.Line)
                {
                    foldingSection = _foldingManager.GetFoldingsContaining(TextArea.Caret.Offset).LastOrDefault();
                }
                if (foldingSection != null)
                {
                    foldingSection.IsFolded = !foldingSection.IsFolded;
                }
            }
        }

        private void ToggleAllFolds()
        {
            if (_foldingManager != null)
            {
                foreach (FoldingSection current in _foldingManager.AllFoldings)
                {
                    current.IsFolded = !current.IsFolded;
                }
            }
        }

        private void ChangeFoldStatus(bool isFolded)
        {
            foreach (FoldingSection current in _foldingManager.AllFoldings)
            {
                current.IsFolded = isFolded;
            }
        }

        private void ShowDefinitions()
        {
            if (_foldingManager != null)
            {
                foreach (FoldingSection current in _foldingManager.AllFoldings)
                {
                    current.IsFolded = current.Tag is NewFolding;
                }
            }
        }

        private void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /*
                private void SetWatcher()
                {
                    string directoryName = Path.GetDirectoryName(Filename);
                    if (directoryName == null || !Directory.Exists(directoryName))
                    {
                    }
                }
        */

        private void Reload()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to reload file?", "Reload file",
                MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
            if (messageBoxResult == MessageBoxResult.OK | !IsModified)
            {
                Load(Filename);
                UpdateFolds();
            }
        }

        /*
                [Localizable(false)]
                private void InsertSnippet()
                {
                    var snippetReplaceableTextElement = new SnippetReplaceableTextElement {Text = "i"};
                    var snippet = new Snippet
                    {
                        Elements =
                        {
                            new SnippetTextElement
                            {
                                Text = "for "
                            },
                            new SnippetReplaceableTextElement
                            {
                                Text = "item"
                            },
                            new SnippetTextElement
                            {
                                Text = " in range("
                            },
                            new SnippetReplaceableTextElement
                            {
                                Text = "from"
                            },
                            new SnippetTextElement
                            {
                                Text = ", "
                            },
                            new SnippetReplaceableTextElement
                            {
                                Text = "to"
                            },
                            new SnippetTextElement
                            {
                                Text = ", "
                            },
                            new SnippetReplaceableTextElement
                            {
                                Text = "step"
                            },
                            new SnippetTextElement
                            {
                                Text = "):backN\t"
                            },
                            new SnippetSelectionElement()
                        }
                    };
                    snippet.Insert(TextArea);
                }
        */

        private void TextEditorGotFocus(object sender, RoutedEventArgs e)
        {
            DocumentViewModel.Instance.TextBox = this;
            OnPropertyChanged("Line");
            OnPropertyChanged("Column");
            OnPropertyChanged("Offset");
            OnPropertyChanged("RobotType");
            FileSave = (!string.IsNullOrEmpty(Filename))
                ? File.GetLastWriteTime(Filename).ToString(CultureInfo.InvariantCulture)
                : string.Empty;
        }

        protected internal virtual char[] GetWordParts() => new char[0];





        public  string GetWordBeforeCaret( char[] allowedChars)
        {
         
            int offset = TextArea.Caret.Offset;

            int num = Document.FindPrevWordStart( offset, allowedChars);
            return num < 0 ? string.Empty : Document.GetText(num, offset - num);
        }

        public  string GetStringBeforeCaret()
        {
           
            int line = TextArea.Caret.Line;
            if (line < 1)
            {
                return string.Empty;
            }
            int offset = TextArea.Caret.Offset;
            if (line >  Document.LineCount)
            {
                return string.Empty;
            }
            DocumentLine lineByNumber = Document.GetLineByNumber(line);
            int length = offset - lineByNumber.Offset;
            return Document.GetText(lineByNumber.Offset, length);
        }


        public  string GetWordBeforeOffset( int offset, char[] allowedChars)
        {
            
            int num = Document.FindPrevWordStart( offset, allowedChars);
            return num < 0 ? string.Empty : Document.GetText(num, offset - num);
        }
        public  string GetTokenBeforeOffset(int offset)
        {
            int num = -1;
            for (int i = offset - 1; i > -1; i--)
            {
                char charAt =   Document.GetCharAt(i);
                if (charAt == ' ' || charAt == '\n' || charAt == '\r' || charAt == '\t')
                {
                    num = i + 1;
                    break;
                }
            }
            return num < 0 ? string.Empty : Document.GetText(num, offset - num);
        }
        public string GetWordUnderCaret(  char[] allowedChars)
        {            
            int offset = TextArea.Caret.Offset;
            int num = Document.FindPrevWordStart( offset, allowedChars);
            int num2 = Document.FindNextWordEnd(offset, allowedChars);
            return num < 0 || num2 == 0 || num2 < num ? string.Empty : Document.GetText(num, num2 - num);
        }
        public string GetFirstWordInLine(int lineNumber) => Document.GetFirstWordInLine(lineNumber);

        public string GetWordUnderCaret()
        {
            
            int offset = TextArea.Caret.Offset;
            int num = Document.FindPrevWordStart(offset);
            int num2 = Document.FindNextWordEnd(offset);
            return num < 0 || num2 == 0 || num2 < num ? string.Empty : Document.GetText(num, num2 - num);
        }
        public  string GetWordUnderOffset(int offset)
        {           
            int num = Document.FindPrevWordStart(offset);
            int num2 = Document.FindNextWordEnd(offset);
            return num < 0 || num2 == 0 || num2 < num ? string.Empty : Document.GetText(num, num2 - num);
        }
        public string GetWordUnderOffset(  int offset, char[] allowedChars)
        {
            int num = Document.FindPrevWordStart( offset, allowedChars);
            int num2 = Document.FindNextWordEnd(offset, allowedChars);
            return num < 0 || num2 == 0 || num2 < num ? string.Empty : Document.GetText(num, num2 - num);
        }

    }
}
