using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RobotEditor.Controls.TextEditor;
using System.Windows.Input;

namespace RobotEditor.ViewModel
{
    public sealed class GotoViewModel : ObservableRecipient
    {
        private RelayCommand _okCommand;

        #region Editor



        private Editor _editor = new Editor();

        /// <summary>
        ///     Sets and gets the Editor property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public Editor Editor
        {
            get => _editor;
            set => SetProperty(ref _editor, value);

        }

        #endregion

        #region Description

        /// <summary>
        ///     The <see cref="Description" /> property's name.
        /// </summary>
        private const string DescriptionPropertyName = "Description";

        private string _description = string.Empty;

        /// <summary>
        ///     Sets and gets the Description property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Description
        {
            get => _description;

            set
            {
                if (_description == value)
                {
                    return;
                }


                _description = value;
                OnPropertyChanged(DescriptionPropertyName);
            }
        }

        #endregion

        #region EnteredText

        /// <summary>
        ///     The <see cref="EnteredText" /> property's name.
        /// </summary>
        private const string EnteredTextPropertyName = "EnteredText";

        private int _enteredText = -1;

        /// <summary>
        ///     Sets and gets the EnteredText property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int EnteredText
        {
            get => _enteredText;

            set
            {
                if (_enteredText == value)
                {
                    return;
                }


                _enteredText = value;
                OnPropertyChanged(EnteredTextPropertyName);
            }
        }

        #endregion

        #region SelectedLine

        /// <summary>
        ///     The <see cref="SelectedLine" /> property's name.
        /// </summary>
        private const string SelectedLinePropertyName = "SelectedLine";

        private int _selectedLine = -1;

        /// <summary>
        ///     Sets and gets the SelectedLine property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public int SelectedLine
        {
            get => _selectedLine;

            set
            {
                if (_selectedLine == value)
                {
                    return;
                }


                _selectedLine = value;
                OnPropertyChanged(SelectedLinePropertyName);
            }
        }

        #endregion

        #region CancelCommand

        private RelayCommand _cancelCommand;

        /// <summary>
        ///     Gets the CancelCommand.
        /// </summary>
        public RelayCommand CancelCommand => _cancelCommand
                       ?? (_cancelCommand = new RelayCommand(ExecuteCancelCommand));

        private void ExecuteCancelCommand()
        {
        }

        #endregion

        // ReSharper restore ExplicitCallerInfoArgument

        public GotoViewModel(Editor editor)
        {
            Editor = editor;
        }

        public GotoViewModel()
        {
        }

        public ICommand OkCommand => _okCommand ?? (_okCommand = new RelayCommand(Accept));

        private void Accept()
        {
            ICSharpCode.AvalonEdit.Document.DocumentLine lineByNumber = Editor.Document.GetLineByNumber(EnteredText);
            Editor.CaretOffset = lineByNumber.Offset;
            Editor.TextArea.Caret.BringCaretToView();
            Editor.ScrollToLine(_selectedLine);
        }
    }
}