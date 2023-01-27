using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.ViewModel;
using System;
using System.Collections.ObjectModel;

namespace RobotEditor.Languages
{
    public sealed class DatCleanHelper : ToolViewModel
    {
        private static DatCleanHelper _instance;
        public static RelayCommand Cleandat;
        private readonly string _filename;
        private bool _commentdeclaration;
        private bool _deletedeclaration;
        private bool _exclusivetypes;
        private bool _ignoretypes;
        private ReadOnlyCollection<IVariable> _listItems;
        private int _progress;

        private int _selectedVarIndex;

        public DatCleanHelper()
            : base("Dat Cleaner")
        {
            Instance = this;
            DefaultPane = DefaultToolPane.Right;
            MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
            _filename = instance.ActiveEditor.FilePath;
            base.Width = 619;
            base.Height = 506;
        }

        public ReadOnlyCollection<IVariable> ListItems
        {
            get
            {
                ReadOnlyCollection<IVariable> _items;
                if ((_items = _listItems) == null)
                {
                    _items =
                        _listItems =
                            Ioc.Default.GetRequiredService<ObjectBrowserViewModel>()
                                .GetVarForFile(KUKA.GetDatFileName(_filename));
                }
                return _items;
            }
        }

        public int Progress { get => _progress; set => SetProperty(ref _progress, value); }

        public static DatCleanHelper Instance
        {
            get
            {
                DatCleanHelper arg_15_0;
                if ((arg_15_0 = _instance) == null)
                {
                    arg_15_0 = _instance = new DatCleanHelper();
                }
                return arg_15_0;
            }
            set => _instance = value;
        }

        public bool IgnoreTypes { get => _ignoretypes; set => SetProperty(ref _ignoretypes, value); }

        public bool ExclusiveTypes { get => _exclusivetypes; set => SetProperty(ref _exclusivetypes, value); }

        public bool DeleteDeclaration { get => _deletedeclaration; set => SetProperty(ref _deletedeclaration, value); }

        public bool CommentDeclaration { get => _commentdeclaration; set => SetProperty(ref _commentdeclaration, value); }

        public int SelectedVarIndex { get => _selectedVarIndex; set => SetProperty(ref _selectedVarIndex, value); }

        public ObservableCollection<string> UsedVarTypes { get; } = new ObservableCollection<string>
        {
            "actual selection",
            "actual dat",
            "all Dat's"
        };

        #region CleanDatCmd

        private static RelayCommand<object> _cleanDatCmd;

        /// <summary>
        ///     Gets the CleanDatCmd.
        /// </summary>
        public static RelayCommand<object> CleanDatCmd => _cleanDatCmd
                       ?? (_cleanDatCmd = new RelayCommand<object>(
                           p => Instance.CleanDat()));

        #endregion

        #region CheckedCmd

        private static RelayCommand _checkedCmd;

        /// <summary>
        ///     Gets the CheckedCmd.
        /// </summary>
        public static RelayCommand CheckedCmd => _checkedCmd
                       ?? (_checkedCmd = new RelayCommand(
                           () => Instance.Checked()));

        #endregion

        #region DeleteVarTypeCmd

        private RelayCommand _deleteVarTypeCmd;

        /// <summary>
        ///     Gets the DeleteVarTypeCmd.
        /// </summary>
        public RelayCommand DeleteVarTypeCmd => _deleteVarTypeCmd
                       ?? (_deleteVarTypeCmd = new RelayCommand(ExecuteDeleteVarTypeCmd));

        private void ExecuteDeleteVarTypeCmd() => Instance.DeleteVarType();

        #endregion

        #region AddVarTypeCmd

        private RelayCommand _addVarTypeCmd;

        /// <summary>
        ///     Gets the AddVarTypeCmd.
        /// </summary>
        public RelayCommand AddVarTypeCmd => _addVarTypeCmd
                       ?? (_addVarTypeCmd = new RelayCommand(ExecuteAddVarTypeCmd));

        private void ExecuteAddVarTypeCmd() => Instance.AddVarType();

        #endregion

        #region SelectAllCommand

        private RelayCommand _selectAllCommand;

        /// <summary>
        ///     Gets the SelectAllCommand.
        /// </summary>
        public RelayCommand SelectAllCommand => _selectAllCommand
                       ?? (_selectAllCommand = new RelayCommand(ExecuteSelectAllCommand));

        private void ExecuteSelectAllCommand() => Instance.SelectAll();

        #endregion

        #region InvertSelectionCommand

        private static RelayCommand _invertSelectionCommand;

        /// <summary>
        ///     Gets the InvertSelectionCommand.
        /// </summary>
        public static RelayCommand InvertSelectionCommand => _invertSelectionCommand
                       ?? (_invertSelectionCommand = new RelayCommand(
                           () => Instance.InvertSelection()));

        #endregion

        public void CleanDat() => throw new NotImplementedException();

        public void Checked() => throw new NotImplementedException();

        public void DeleteVarType() => throw new NotImplementedException();

        public void AddVarType() => throw new NotImplementedException();

        private void SelectAll()
        {
            foreach (IVariable current in ListItems)
            {
                current.IsSelected = true;
            }
            OnPropertyChanged(nameof(IgnoreTypes));
        }

        private void InvertSelection()
        {
            foreach (IVariable current in ListItems)
            {
                current.IsSelected = !current.IsSelected;
            }
            OnPropertyChanged(nameof(IgnoreTypes));
        }

        public void Clean()
        {
        }
    }
}