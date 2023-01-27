

using CommunityToolkit.Mvvm.ComponentModel;

namespace RobotEditor.ViewModel
{
    internal sealed class AdjustST : ObservableObject
    {
        #region ToolItems

        /// <summary>
        ///     The <see cref="ToolItems" /> property's name.
        /// </summary>
        private const string ToolItemsPropertyName = "ToolItems";

        private ToolItems _toolItems = new ToolItems();

        /// <summary>
        ///     Sets and gets the ToolItems property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public ToolItems ToolItems
        {
            get => _toolItems;
            set => SetProperty(ref _toolItems, value);

        }

        #endregion
    }
}