using System.Windows.Media.Imaging;
using RobotEditor.Interfaces;
using RobotEditor.Messages;

namespace RobotEditor.Classes
{
    public sealed class OutputWindowMessage : MessageBase, IMessage
    {
        public string Time { get; set; }
        
    }
}