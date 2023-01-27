using System;
using System.Windows;

namespace RobotEditor.Windows
{
    /// <summary>
    ///     Interaction logic for FindAndReplaceWindow.xaml
    /// </summary>
    public sealed partial class FindAndReplaceWindow : Window
    {
        private static FindAndReplaceWindow _instance;

        public FindAndReplaceWindow()
        {
            InitializeComponent();
        }

        public FindAndReplaceWindow(MainWindow instance)
        {
            throw new NotImplementedException();
        }

        public static FindAndReplaceWindow Instance
        {
            get => _instance ?? (_instance = new FindAndReplaceWindow());
            set => _instance = value;
        }
    }
}