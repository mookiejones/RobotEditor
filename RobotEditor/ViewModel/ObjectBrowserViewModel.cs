using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.DependencyInjection;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Utilities;

namespace RobotEditor.ViewModel
{
    public sealed class ObjectBrowserViewModel : ToolViewModel
    {
        private const string ToolContentId = "ObjectBrowserTool";
        private int _progress;
        private int _progressMax;
        private IVariable _selectedVariable;

        #region Properties

        public IVariable SelectedVariable
        {
            get => _selectedVariable;
            set
            {
                _selectedVariable = value;
                var instance = Ioc.Default.GetRequiredService<MainViewModel>();
                instance.OpenFile(value);
                OnPropertyChanged(nameof(SelectedVariable));
            }
        }

        public int Progress { get =>_progress; set=>SetProperty(ref _progress,value); }

        public int ProgressMax { get =>_progressMax; set=>SetProperty(ref _progressMax,value); }

        #endregion

        public ObjectBrowserViewModel()
            : base("Object Browser")
        {
            DefaultPane = DefaultToolPane.Bottom;
            Initialize();
        }

        private string GetDirectory()
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = @"Select Root Directory for Instance"
            };
            var instance = Ioc.Default.GetRequiredService<MainViewModel>();
            var filename = instance.ActiveEditor.TextBox.Filename;
            if (Directory.Exists(filename))
            {
                folderBrowserDialog.SelectedPath = Path.GetDirectoryName(filename);
            }
            return (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                ? folderBrowserDialog.SelectedPath
                : string.Empty;
        }

        private void Initialize()
        {
            ContentId = ToolContentId;
            IconSource=ImageHelper.GetIcon(Global.IconObjectBrowser);
            //  IconSource = Utilities.GetIcon("pack://application:,,/Images/resources-objectbrowser.png");
            DefaultPane = DefaultToolPane.Bottom;
        }

        public ReadOnlyCollection<IVariable> GetVarForFile(string filename)
        {
            var instance = Ioc.Default.GetRequiredService<MainViewModel>();
            ReadOnlyCollection<IVariable> result;
            if (!(instance.ActiveEditor is KukaViewModel kukaViewModel))
            {
                result = null;
            }
            else
            {
                var variables = kukaViewModel.Data.Variables;
                var list = (
                    from p in variables
                    where p.Type == "e6pos"
                    select p).ToList<IVariable>();
                result = new ReadOnlyCollection<IVariable>(list);
            }
            return result;
        }
    }
}