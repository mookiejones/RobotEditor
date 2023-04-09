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

        private string _appName => Assembly.GetExecutingAssembly().GetName().Name;

        private string _version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private string _newVersion => Environment.Version.ToString();

        private string _osVersion => Environment.OSVersion.ToString();

        private string _currentCulture => CultureInfo.CurrentCulture.Name;

        private string _workingSetMemory => GC.GetTotalMemory(true).ToString(CultureInfo.InvariantCulture);

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
                StringBuilder stringBuilder = new();
                _ = stringBuilder.AppendLine(" Name                : " + _appName);
                _ = stringBuilder.AppendLine(" .Net Version        : " + _newVersion);
                _ = stringBuilder.AppendLine(" Version        : " + _version);
                _ = stringBuilder.AppendLine(" OS Version          : " + _osVersion);
                _ = stringBuilder.AppendLine(" Current Culture     : " + _currentCulture);
                _ = stringBuilder.AppendLine(" Working Set Memory  : " + _workingSetMemory);
                return stringBuilder.ToString();
            }
        }
    }
}