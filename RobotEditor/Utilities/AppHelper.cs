using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace RobotEditor.Utilities
{
    public static class AppHelper
    {
        private const string CHECK_ENVIRONMENT = "This version of {0} requires .Net 4.0. Your are using:{1}";
        private const string WINDIR = "WINDIR";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        [Localizable(false)]
        public static bool CheckEnvironment(this Application app)
        {
            // Safety check: our setup already checks that .NET 4 is installed, but we manually check the .NET version in case SharpDevelop is
            // used on another machine than it was installed on (e.g. "SharpDevelop on USB stick")
            if (Environment.Version < new Version(4, 0, 30319))
            {
                string name = Assembly.GetExecutingAssembly().GetName().Name;
                Version version = Environment.Version;

                _ = MessageBox.Show(string.Format(CHECK_ENVIRONMENT, name, version));
                return false;
            }
            // Work around a WPF issue when %WINDIR% is set to an incorrect path
            string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows,
                Environment.SpecialFolderOption.DoNotVerify);
            if (Environment.GetEnvironmentVariable(WINDIR) != windir)
            {
                Environment.SetEnvironmentVariable(WINDIR, windir);
            }
            return true;
        }
    }
}
