using CommunityToolkit.Mvvm.ComponentModel;

namespace RobotEditor.ViewModel
{
    public sealed class ToolTipViewModel :  ObservableRecipient
    {
        private static ToolTipViewModel _instance;
        private string _message = string.Empty;
        private string _title = string.Empty;

        public static ToolTipViewModel Instance
        {
            get => _instance ?? new ToolTipViewModel();
            set => _instance = value;
        }

        public string Message { get =>_message; set=>SetProperty(ref _message,value); }

        public string Title { get =>_title; set=>SetProperty(ref _title,value); }
    }
}