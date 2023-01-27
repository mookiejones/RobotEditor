using System.Diagnostics;

namespace RobotEditor.Controls.FTP
{
    public class FTPFolder
    {
        public string Path { get; set; }
        public string Name { get; set; }

        [DebuggerStepThrough]
        public static string SafeFolderName(string path)
        {
            var array = path.Split(new[]
            {
                '/'
            });
            return array[array.Length - 1];
        }
    }
}