using RobotEditor.Utilities;
using RobotEditor.ViewModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Media;

namespace RobotEditor
{
    public static class Global
    {
        #region Image Paths

        public const string DockConfigPath = "dockConfig.xml";
        public const string LogFile = "logFile.txt";
        public const string ImgError = "..\\..\\Images\\resources-error.png";
        public const string ImgInfo = "..\\..\\Images\\resources-info.png";
        public const string IconObjectBrowser = "pack://application:,,,/Images/resources-objectbrowser.png";
        public const string IconProperty = "pack://application:,,/Resources/property-blue.png";
        public const string ImgConst = "..\\..\\Images\\resources-vxconstant_icon.png";
        public const string ImgStruct = "..\\..\\Images\\resources-vxstruct_icon.png";
        public const string ImgMethod = "..\\..\\Images\\resources-vxmethod_icon.png";
        public const string ImgEnum = "..\\..\\Images\\resources-vxenum_icon.png";
        public const string ImgField = "..\\..\\Images\\resources-vxfield_icon.png";
        public const string ImgValue = "..\\..\\Images\\resources-vxvaluetype_icon.png";
        public const string ImgSignal = "..\\..\\Images\\resources-vxevent_icon.png";
        public const string ImgXyz = "..\\..\\Images\\resources-vxXYZ_icon.png";
        public const string ImgSrc = "..\\..\\Images\\resources-srcfile.png";
        public const string ImgDat = "..\\..\\Images\\resources-datfile.png";
        public const string ImgSps = "..\\..\\Images\\resources-spsfile.png";

        #endregion

        public static string StartupPath => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static string Version
        {
            get
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
                return executingAssembly.GetName().Version.ToString();
            }
        }

        public static string ProductName => Assembly.GetExecutingAssembly().GetName().ToString();

        [Localizable(false)]
        public static string DockConfig => AppDomain.CurrentDomain.BaseDirectory + "dockConfig.xml";

        public static bool DoesDirectoryExist(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            bool result;
            if (fileInfo.DirectoryName != null)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(fileInfo.DirectoryName);
                try
                {
                    if (Directory.GetDirectories(directoryInfo.Root.ToString()).Length > 0)
                    {
                        result = true;
                        return result;
                    }
                }
                catch
                {
                    result = false;
                    return result;
                }
            }
            result = false;
            return result;
        }

        public static void WriteLog(string message)
        {
            LogWriter.WriteLog(message);
        }

        public static void ErrorHandler(string message)
        {
            ErrorHandler(message, false);
        }

        private static void ErrorHandler(string message, bool showmessage)
        {
            Console.WriteLine(message);
            TraceWriter.Trace(message);
            LogWriter.WriteLog(message, showmessage ? Colors.Red : Colors.Gray);
            if (showmessage)
            {
                MessageViewModel.ShowMessage(message);
            }
        }
    }
}