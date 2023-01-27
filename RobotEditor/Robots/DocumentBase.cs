using System;
using System.IO;
using System.Windows;
using RobotEditor.Abstract;
using RobotEditor.Controls.TextEditor;
using RobotEditor.Interfaces;
using RobotEditor.Languages;
using RobotEditor.ViewModel;

namespace RobotEditor.Classes
{
    public abstract class DocumentBase : FileViewModel, IEditorDocument
    {
        public DocumentBase()
        {
        }

        public DocumentBase(string filePath) : base(filePath)
        {
            ContentId = filePath;
            InitializeControl();
        }

        public DocumentBase(string filePath, AbstractLanguageClass language) : base(filePath)
        {
            ContentId = filePath;
            FileLanguage = language;
            InitializeControl();
        }


        public abstract void Close();

        public abstract void Load(string filePath);

        public void SelectText(IVariable variable)
        {
            if (variable.Name == null) throw new ArgumentNullException(nameof(variable));

            //TODO Need to find out if this will work from Global Variables. Only Tested so far for Local Variable Window

            // Is Offset of textbox greater than desired value?
            var enoughLines = TextBox.Text.Length >= variable.Offset;
            if (enoughLines)
                    TextBox.SelectText(variable);
        }

        private void InitializeControl()
        {
            TextBox.FileLanguage = FileLanguage;
            Load(ContentId);
           
            TextBox.GotFocus += delegate(object s, RoutedEventArgs e) { TextBox = (s as Editor); };
            TextBox.TextChanged += (s, e) => TextChanged(s);
            TextBox.IsModified = false;
            if (ContentId != null)
            {
                FileLanguage.GetRootDirectory(Path.GetDirectoryName(ContentId));
            }
            TextBox.TextChanged += (s, e) => TextChanged(s);
            TextBox.IsModified = false;
        }

        protected void TextChanged(object sender)
        {
            TextBox = (sender as Editor);
            if (TextBox != null)
            {
                FileLanguage.RawText = TextBox.Text;
            }
            OnPropertyChanged(nameof(Title));
        }

        internal void Save(Editor txtBox)
        {
            if (txtBox.Filename == null)
            {
                txtBox.SaveAs();
            }
            else
            {
                txtBox.Save(FileName);
            }
            IsDirty = false;
        }

        #region FileLanguage

        private AbstractLanguageClass _filelanguage = new LanguageBase();

        public AbstractLanguageClass FileLanguage { get =>_filelanguage; set=>SetProperty(ref _filelanguage,value); }

        #endregion

        #region Visibility

        private Visibility _visibility = Visibility.Visible;

        public Visibility Visibility { get =>_visibility; set=>SetProperty(ref _visibility,value); }

        #endregion        public AbstractLanguageClass FileLanguage { get; set; }

        #region TextBox

        /// <summary>
        ///     The <see cref="TextBox" /> property's name.
        /// </summary>
        private const string TextBoxPropertyName = "TextBox";

        private Editor _textBox = new Editor();

        /// <summary>
        ///     Sets and gets the TextBox property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public Editor TextBox
        {
            get => _textBox;

            set
            {
                if (_textBox == value)
                {
                    return;
                }


                _textBox = value;
                OnPropertyChanged(TextBoxPropertyName);
            }
        }

        #endregion
    }
}