using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace RobotEditor.ViewModel
{
    public class PaneViewModel : ObservableRecipient
    {
        private string _contentId;
        private bool _isActive;
        private bool _isSelected;
        private string _title;

        public string Title { get => _title; set => SetProperty(ref _title, value); }


        public ImageSource IconSource { get; set; }

        public string ContentId { get => _contentId; set => SetProperty(ref _contentId, value); }


        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }

        public new bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
    }
}