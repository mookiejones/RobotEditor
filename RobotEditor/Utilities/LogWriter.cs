using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Media;

namespace RobotEditor.Utilities
{
    public sealed class LogWriter
    {
        [Localizable(false)]
        public LogWriter()
        {
            File.Delete(string.Format("{0}{1}", Global.StartupPath, "\\KRC Editor.log"));
            WriteLog(string.Format("{0} {1} Created", Global.ProductName, Global.Version));
        }

        public static void WriteLog(string message, Exception ex)
        {
            WriteLog(message, Colors.Red);
            WriteLog(ex.Message, Colors.Red);
        }

        [Localizable(false)]
        public static void WriteLog(string message, Color color)
        {
            string path = string.Format("{0}{1}", Global.StartupPath, "\\KRC Editor.log");
            File.AppendAllText(path, message + "/r/n", Encoding.Default);
        }

        public static void WriteLog(string message)
        {
            WriteLog(message, Colors.Gray);
        }
    }
}