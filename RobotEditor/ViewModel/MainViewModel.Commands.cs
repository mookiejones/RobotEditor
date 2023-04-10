using CommunityToolkit.Mvvm.Input;
using RobotEditor.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotEditor.ViewModel;

    public partial class MainViewModel
    {


    [RelayCommand]
    private void ExecuteCloseCommand(object obj)

    {
        _ = _files.Remove(ActiveEditor);

        ActiveEditor.Close();
        ActiveEditor = _files.FirstOrDefault();
        // Close(ActiveEditor);
        OnPropertyChanged(nameof(ActiveEditor));
    }


    #region Commands

  

    #region ShowIOCommand

    private RelayCommand _showIOCommand;

    /// <summary>
    ///     Gets the ShowIOCommand.
    /// </summary>
    public RelayCommand ShowIOCommand => _showIOCommand ??= new RelayCommand(ExecuteShowIO);

    #endregion

    #region ChangeThemeCommand

    private RelayCommand<object> _changeThemeCommand;

    /// <summary>
    ///     Gets the ChangeThemeCommand.
    /// </summary>
    public RelayCommand<object> ChangeThemeCommand => _changeThemeCommand ??= new RelayCommand<object>(
                       ChangeTheme);

    #endregion

    #region ChangeAccentCommand

    private RelayCommand<object> _changeAccentCommand;

    /// <summary>
    ///     Gets the ChangeAccentCommand.
    /// </summary>
    public RelayCommand<object> ChangeAccentCommand => _changeAccentCommand ??= new RelayCommand<object>(
                       ChangeAccent);

    #endregion

    #region NewFileCommand

    private RelayCommand _newFileCommand;

    /// <summary>
    ///     Gets the NewFileCommand.
    /// </summary>
    public RelayCommand NewFileCommand => _newFileCommand ??= new RelayCommand(
                       AddNewFile);

    #endregion

   

   

    #region ChangeViewAsCommand

    private RelayCommand<string> _changeViewAsCommand;

    /// <summary>
    ///     Gets the ChangeViewAsCommand.
    /// </summary>
    public RelayCommand<string> ChangeViewAsCommand => _changeViewAsCommand ??= new RelayCommand<string>(ChangeViewAs);

    #endregion

    #region ExitCommand

    private RelayCommand _exitCommand;

    /// <summary>
    ///     Gets the ExitCommand.
    /// </summary>
    public RelayCommand ExitCommand => _exitCommand ??= new RelayCommand(Exit);

    #endregion

    #region ImportCommand

    private RelayCommand<object> _importCommand;

    /// <summary>
    ///     Gets the ImportCommand.
    /// </summary>
    public RelayCommand<object> ImportCommand => _importCommand ??= new RelayCommand<object>(p => ImportRobot(), CanImport);

    public bool CanImport(object p) => !((p is LanguageBase) | p is Fanuc | p is Kawasaki | p == null);

    #endregion

    #region AddToolCommand

    private RelayCommand<object> _addToolCommand;

    /// <summary>
    ///     Gets the AddToolCommand.
    /// </summary>
    public RelayCommand<object> AddToolCommand => _addToolCommand ??= new RelayCommand<object>(AddTool);

    #endregion

    #region ShowAboutCommand

    private RelayCommand _showAboutCommand;

    /// <summary>
    ///     Gets the ShowAboutCommand.
    /// </summary>
    public RelayCommand ShowAboutCommand => _showAboutCommand ??= new RelayCommand(ShowAbout);

    #endregion

    #region OpenFileCommand

    private RelayCommand<object> _openFileCommand;

    /// <summary>
    ///     Gets the OpenFileCommand.
    /// </summary>
    public RelayCommand<object> OpenFileCommand => _openFileCommand ??= new RelayCommand<object>(OnOpen);

    #endregion

    #endregion

}

