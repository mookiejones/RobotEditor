using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;

namespace RobotEditor.Windows
{
    /// <summary>
    ///     Interaction logic for AboutWindow.xaml
    /// </summary>
    public sealed partial class AboutWindow : Window
    {

        #region Properties
        private  string Copyright => "Copyright 2023 Charles Berman";

        private string _appName => Assembly.GetExecutingAssembly().GetName().Name;

        private string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private string _newVersion => Environment.Version.ToString();

        private  string _osVersion => Environment.OSVersion.ToString();

        private  string _currentCulture => CultureInfo.CurrentCulture.Name;

        private  string _workingSetMemory => GC.GetTotalMemory(true).ToString(CultureInfo.InvariantCulture);

        #endregion
        public AboutWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void CloseClick(object sender, RoutedEventArgs e) => Close();

        public string Info
        {
            get
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(" Name                : " + _appName);
                stringBuilder.AppendLine(" .Net Version        : " + _newVersion);
                stringBuilder.AppendLine(" OS Version          : " + _osVersion);
                stringBuilder.AppendLine(" Current Culture     : " + _currentCulture);
                stringBuilder.AppendLine(" Working Set Memory  : " + _workingSetMemory);
                return stringBuilder.ToString();
            }
        }
    }
}