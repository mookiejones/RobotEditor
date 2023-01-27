
using CommunityToolkit.Mvvm.ComponentModel;

namespace RobotEditor.ViewModel
{
    public sealed class CartesianPosition : ObservableObject
    {
        #region Header

        /// <summary>
        ///     The <see cref="Header" /> property's name.
        /// </summary>
        private const string HeaderPropertyName = "Header";

        private string _header = string.Empty;

        /// <summary>
        ///     Sets and gets the Header property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Header
        {
            get => _header;

            set => SetProperty(ref _header, value); 
        }

        #endregion

        #region X

        /// <summary>
        ///     The <see cref="X" /> property's name.
        /// </summary>
        private const string XPropertyName = "X";

        private double _x;

        /// <summary>
        ///     Sets and gets the X property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double X
        {
            get => _x;

            set
            {
                if (_x == value)
                {
                    return;
                }


                _x = value;
                OnPropertyChanged(XPropertyName);
            }
        }

        #endregion

        #region Y

        /// <summary>
        ///     The <see cref="Y" /> property's name.
        /// </summary>
        private const string YPropertyName = "Y";

        private double _y;

        /// <summary>
        ///     Sets and gets the Y property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double Y
        {
            get => _y;

            set
            {
                if (_y == value)
                {
                    return;
                }


                _y = value;
                OnPropertyChanged(YPropertyName);
            }
        }

        #endregion

        #region Z

        /// <summary>
        ///     The <see cref="Z" /> property's name.
        /// </summary>
        private const string ZPropertyName = "Z";

        private double _z;

        /// <summary>
        ///     Sets and gets the Z property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public double Z
        {
            get => _z;

            set
            {
                if (_z == value)
                {
                    return;
                }


                _z = value;
                OnPropertyChanged(ZPropertyName);
            }
        }

        #endregion
    }
}