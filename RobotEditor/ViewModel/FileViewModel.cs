using System.IO;

namespace RobotEditor.ViewModel
{
    public abstract class FileViewModel : PaneViewModel
    {
        #region FilePath

        /// <summary>
        ///     The <see cref="FilePath" /> property's name.
        /// </summary>
        private const string FilePathPropertyName = "FilePath";

        private string _filePath = string.Empty;

        /// <summary>
        ///     Sets and gets the FilePath property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string FilePath
        {
            get => _filePath;

            set
            {
                if (_filePath == value)
                {
                    return;
                }

                // ReSharper disable once ExplicitCallerInfoArgument

                _filePath = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(FilePathPropertyName);
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(FileName));
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(Title));
                if (File.Exists(_filePath))
                {
                    ContentId = _filePath;
                }
            }
        }

        #endregion

        #region FileName

        /// <summary>
        ///     Sets and gets the FileName property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string FileName
        {
            get
            {
                string result = FilePath == null ? "Noname" + (IsDirty ? "*" : "") : Path.GetFileName(FilePath) + (IsDirty ? "*" : "");
                return result;
            }
        }

        #endregion

        #region IsDirty

        /// <summary>
        ///     The <see cref="IsDirty" /> property's name.
        /// </summary>
        private const string IsDirtyPropertyName = "IsDirty";

        private bool _isDirty;

        /// <summary>
        ///     Sets and gets the IsDirty property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;

            set
            {
                if (_isDirty == value)
                {
                    return;
                }

                // ReSharper disable once ExplicitCallerInfoArgument

                _isDirty = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(IsDirtyPropertyName);
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(FileName));
            }
        }

        #endregion

        /*
                private static ImageSourceConverter _isc = new ImageSourceConverter();
        */

        public FileViewModel(string filePath)
        {
            FilePath = filePath;
            Title = FileName;
        }

        public FileViewModel()
        {
            IsDirty = true;
            Title = FileName;
        }
    }
}