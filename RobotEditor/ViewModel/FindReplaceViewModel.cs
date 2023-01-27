using System;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace RobotEditor.ViewModel
{
    public sealed class FindReplaceViewModel : ObservableObject
    {
        // ReSharper disable UnusedField.Compiler
        private static RelayCommand _findpreviouscommand;
        private static RelayCommand _findnextcommand;
        private static RelayCommand _replacecommand;
        private static RelayCommand _replaceallcommand;
        private static RelayCommand _highlightallcommand;
        private static FindReplaceViewModel _instance;
        private RelayCommand _findallcommand;
        private string _lookfor = string.Empty;
        private bool _matchcase;
        private bool _matchwholeword;
        private string _replacewith = string.Empty;
        private string _searchresult = string.Empty;
        private bool _useregex;

        #region FindPreviousCommand

        private RelayCommand _findPreviousCommand;

        /// <summary>
        ///     Gets the FindPreviousCommand.
        /// </summary>
        public RelayCommand FindPreviousCommand => _findPreviousCommand
                       ?? (_findPreviousCommand = new RelayCommand(FindPrevious));

        #endregion

        #region

        private RelayCommand _findNextCommand;

        /// <summary>
        ///     Gets the FindNextCommand.
        /// </summary>
        public RelayCommand FindNextCommand => _findNextCommand
                       ?? (_findNextCommand = new RelayCommand(FindNext));

        #endregion

        #region

        private static RelayCommand _replaceCommand;

        /// <summary>
        ///     Gets the ReplaceCommand.
        /// </summary>
        public static RelayCommand ReplaceCommand => _replaceCommand
                       ?? (_replaceCommand = new RelayCommand(Replace));

        #endregion

        #region

        private static RelayCommand _replaceAllCommand;

        /// <summary>
        ///     Gets the ReplaceAllCommand.
        /// </summary>
        public static RelayCommand ReplaceAllCommand => _replaceAllCommand
                       ?? (_replaceAllCommand = new RelayCommand(ReplaceAll));

        #endregion

        #region

        private static RelayCommand _highlightAllCommand;

        /// <summary>
        ///     Gets the HighlightAllCommand.
        /// </summary>
        public static RelayCommand HighlightAllCommand => _highlightAllCommand
                       ?? (_highlightAllCommand = new RelayCommand(
                           HighlightAll));

        #endregion

        #region

        private RelayCommand _findAllCommand;

        /// <summary>
        ///     Gets the FindAllCommand.
        /// </summary>
        public RelayCommand FindAllCommand => _findAllCommand
                       ?? (_findAllCommand = new RelayCommand(
                           FindAll));

        #endregion

        // ReSharper restore UnusedField.Compiler

        public static FindReplaceViewModel Instance
        {
            get
            {
                FindReplaceViewModel arg_15_0;
                if ((arg_15_0 = _instance) == null)
                {
                    arg_15_0 = (_instance = new FindReplaceViewModel());
                }
                return arg_15_0;
            }
            set => _instance = value;
        }

        public bool UseRegex { get =>_useregex; set=>SetProperty(ref _useregex,value); }

        public bool MatchCase { get =>_matchcase; set=>SetProperty(ref _matchcase,value); }

        public bool MatchWholeWord { get =>_matchwholeword; set=>SetProperty(ref _matchwholeword,value); }

        public Regex RegexPattern
        {
            get
            {
                var pattern = (!UseRegex) ? Regex.Escape(LookFor) : LookFor;
                var options = MatchCase ? 0 : 1;
                return new Regex(pattern, (RegexOptions) options);
            }
        }

        public string RegexString => (!UseRegex) ? Regex.Escape(LookFor) : LookFor;

        public string LookFor { get =>_lookfor; set=>SetProperty(ref _lookfor,value); }

        public string ReplaceWith { get =>_replacewith; set=>SetProperty(ref _replacewith,value); }

        public string SearchResult { get =>_searchresult; set=>SetProperty(ref _searchresult,value); }

        private static void FindPrevious() => throw new NotImplementedException();

        private static void FindNext()
        {
            var instance = Ioc.Default.GetRequiredService<MainViewModel>();
            instance.ActiveEditor.TextBox.FindText();
        }

        private static void Replace()
        {
            var instance = Ioc.Default.GetRequiredService<MainViewModel>();
            instance.ActiveEditor.TextBox.ReplaceText();
        }

        private static void ReplaceAll() => throw new NotImplementedException();

        private static void HighlightAll() => throw new NotImplementedException();

        private static void FindAll() => throw new NotImplementedException();
    }
}