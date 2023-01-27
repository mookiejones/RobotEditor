using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Messages;
using RobotEditor.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RobotEditor.ViewModel
{
    public delegate void MessageAddedHandler(object sender, EventArgs e);
    public sealed class MessageViewModel : ToolViewModel
    {
        private const string ToolContentId = "MessageViewTool";
        public event MessageAddedHandler MessageAdded;

        #region Properties
        private static MessageViewModel _instance;
        public static MessageViewModel Instance
        {
            get => _instance ?? new MessageViewModel();
            private set => _instance = value;
        }


        #region SelectedMessage
        /// <summary>
        /// The <see cref="SelectedMessage" /> property's name.
        /// </summary>
        public const string SelectedMessagePropertyName = "SelectedMessage";

        private OutputWindowMessage _selectedMessage = null;

        /// <summary>
        /// Sets and gets the SelectedMessage property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public OutputWindowMessage SelectedMessage
        {
            get => _selectedMessage;

            set => SetProperty(ref _selectedMessage, value);
        }
        #endregion


        public ObservableCollection<IMessage> Messages { get; set; }

        #endregion

        private void RaiseMessageAdded() => MessageAdded?.Invoke(this, new EventArgs());

        #region Constructor
        public MessageViewModel() : base("Output Window")
        {
            ContentId = ToolContentId;
            DefaultPane = DefaultToolPane.Bottom;
            Messages = new ObservableCollection<IMessage>();
            Instance = this;

            WeakReferenceMessenger.Default.Register<Exception>(this, GetException);
        }

        private void GetException(object sender, Exception obj) => throw new NotImplementedException();

        #endregion

        public static void Add(IMessage msg) => Add(msg.Title, msg.Description, msg.Icon);

        public void Add(string title, string message, MsgIcon icon, bool forceactivate = true)
        {
            BitmapImage img = null;

            switch (icon)
            {
                case MsgIcon.Error:
                    img = ImageHelper.LoadBitmap(Global.ImgError);
                    break;
                case MsgIcon.Info:
                    img = ImageHelper.LoadBitmap(Global.ImgInfo);
                    break;
            }


            Messages.Add(new OutputWindowMessage { Title = title, Description = message, Icon = img });

            if (forceactivate)
            {
                RaiseMessageAdded();
            }
        }

        private void HandleMouseOver(object param) => SelectedMessage = (OutputWindowMessage)((ListViewItem)param).Content;

        /// <summary>
        /// Create MessageBox window and displays
        /// </summary>
        /// <param name="message"></param>
        public static void ShowMessage(string message) => _ = System.Windows.MessageBox.Show(message);

        private void ClearItems()
        {
            Messages.Clear();//=new ObservableCollection<OutputWindowMessage>();

            OnPropertyChanged("Messages");
        }

        public static void AddError(string message, Exception ex)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            OutputWindowMessage msg = new OutputWindowMessage
            {
                Title = "Internal Error",
                Icon = ImageHelper.LoadBitmap(Global.ImgError),
                Description = string.Format("Internal error\r\n {0} \r\n in {1}", ex.Message, trace.GetFrame(2))
            };
            //            msg.Icon = (BitmapImage)Application.Current.Resources.MergedDictionaries[0]["error"];

            Instance.Messages.Add(msg);

        }

        public static void Add(string title, string message, BitmapImage icon, bool forceactivate = true)
        {

            Instance.Messages.Add(new OutputWindowMessage { Title = title, Description = message, Icon = icon });

            if (forceactivate)
            {
                Instance.RaiseMessageAdded();
            }
        }

        #region Commands


        #region ClearMessagesCommand
        private RelayCommand _clearMessagesCommand;

        /// <summary>
        /// Gets the ClearMessagesCommand.
        /// </summary>
        public RelayCommand ClearMessagesCommand => _clearMessagesCommand
                    ?? (_clearMessagesCommand = new RelayCommand(ClearItems));

        #endregion





        #region MouseOverCommand
        private RelayCommand<object> _mouseOverCommand;

        /// <summary>
        /// Gets the MouseOverCommand.
        /// </summary>
        public RelayCommand<object> MouseOverCommand => _mouseOverCommand
                    ?? (_mouseOverCommand = new RelayCommand<object>(HandleMouseOver));


        #endregion
        #endregion

    }
}