using RobotEditor.Interfaces;

namespace RobotEditor.Messages;

public sealed class OutputWindowMessage : MessageBase, IMessage
{
    public string Time { get; set; }

}