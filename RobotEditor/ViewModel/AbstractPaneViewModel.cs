using CommunityToolkit.Mvvm.ComponentModel;

namespace RobotEditor.ViewModel
{
    public abstract class AbstractPaneViewModel : ObservableRecipient
    {
        private bool _isVisible = true;

        public bool IsVisible { get =>_isVisible; set=>SetProperty(ref _isVisible,value); }
    }
}