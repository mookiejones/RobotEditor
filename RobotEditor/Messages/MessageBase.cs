using System.Windows.Media.Imaging;
using RobotEditor.Classes;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Utilities;

namespace RobotEditor.Messages
{
    public class MessageBase :   IMessage
    {

        protected MessageBase() { }
        protected MessageBase(string title, string description, MessageType icon, bool force = false)
        {
            Title = title;
            Description = description;
            Icon = GetIcon(icon);
            ForceActivation = force;
        }

        public BitmapImage Icon { get;  set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool ForceActivation { get; set; }

        private BitmapImage GetIcon(MessageType icon)
        {
            switch (icon)
            {
                case MessageType.Error:
                    return ImageHelper.LoadBitmap(Global.ImgError);
                case MessageType.Information:
                    return ImageHelper.LoadBitmap(Global.ImgInfo);
            }
            return null;
        }
    }
}