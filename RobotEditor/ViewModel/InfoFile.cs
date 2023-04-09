using CommunityToolkit.Mvvm.ComponentModel;
using RobotEditor.Model;
using System.Collections.ObjectModel;

namespace RobotEditor.ViewModel
{
    public sealed class InfoFile : ObservableRecipient
    {
        private readonly ReadOnlyObservableCollection<Technology> _readonlyTechnology = null;
        private readonly ObservableCollection<Technology> _technologies = new();
        private string _archiveDate = string.Empty;
        private string _archiveDiskNo = string.Empty;
        private string _archiveID = string.Empty;
        private string _archiveKssVersion = string.Empty;
        private string _archiveRobotName = string.Empty;
        private string _archiveRobotSerial = string.Empty;
        private string _archiveconfigtype = string.Empty;
        private string _archivename = string.Empty;

        public string ArchiveName { get => _archivename; set => SetProperty(ref _archivename, value); }

        public string ArchiveConfigType { get => _archiveconfigtype; set => SetProperty(ref _archiveconfigtype, value); }

        public string ArchiveDiskNo { get => _archiveDiskNo; set => SetProperty(ref _archiveDiskNo, value); }

        public string ArchiveID { get => _archiveID; set => SetProperty(ref _archiveID, value); }

        public string ArchiveDate { get => _archiveDate; set => SetProperty(ref _archiveDate, value); }

        public string RobotName { get => _archiveRobotName; set => SetProperty(ref _archiveRobotName, value); }

        public string RobotSerial { get => _archiveRobotSerial; set => SetProperty(ref _archiveRobotSerial, value); }

        public string KSSVersion { get => _archiveKssVersion; set => SetProperty(ref _archiveKssVersion, value); }

        public ReadOnlyObservableCollection<Technology> Technologies => _readonlyTechnology ?? new ReadOnlyObservableCollection<Technology>(_technologies);
    }
}