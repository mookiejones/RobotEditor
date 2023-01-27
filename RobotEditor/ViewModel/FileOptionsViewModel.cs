
using CommunityToolkit.Mvvm.ComponentModel;

namespace RobotEditor.ViewModel
{
    public sealed class FileOptionsViewModel : ObservableObject
    {
        #region ShowFullName

        /// <summary>
        ///     The <see cref="ShowFullName" /> property's name.
        /// </summary>
        private const string ShowFullNamePropertyName = "ShowFullName";

        private bool _showFullName = true;

        /// <summary>
        ///     Sets and gets the ShowFullName property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public bool ShowFullName
        {
            get => _showFullName;

            set
            {
                if (_showFullName == value)
                {
                    return;
                }


                _showFullName = value;
                OnPropertyChanged(ShowFullNamePropertyName);
            }
        }

        #endregion
    }
}