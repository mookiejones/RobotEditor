using RobotEditor.Enums;

namespace RobotEditor.ViewModel
{
    public sealed class NotesViewModel : ToolViewModel
    {
        #region Text

         

        private string _text = string.Empty;

        /// <summary>
        ///     Sets and gets the Text property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Text
        {
            get => _text;

            set
            {
                if (_text == value)
                {
                    return;
                }


                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        #endregion

        public NotesViewModel()
            : base("Notes")
        {
            ContentId = "NotesTool";
            DefaultPane = DefaultToolPane.Bottom;
        }
    }
}