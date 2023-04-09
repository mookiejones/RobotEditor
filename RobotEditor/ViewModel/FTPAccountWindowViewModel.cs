using RobotEditor.Controls.FTP;
using System.Collections.ObjectModel;

namespace RobotEditor.ViewModel
{
    // ReSharper disable once UnusedMember.Global
    public class FTPAccountWindowViewModel : ToolViewModel
    {
        private readonly ObservableCollection<FTPAccount> _accounts = new();
        private readonly ReadOnlyObservableCollection<FTPAccount> _readonlyAccounts = null;

        public FTPAccountWindowViewModel()
            : base("FTP Accounts")
        {
        }

        public ReadOnlyObservableCollection<FTPAccount> Accounts => _readonlyAccounts ?? new ReadOnlyObservableCollection<FTPAccount>(_accounts);
    }
}