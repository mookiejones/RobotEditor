using System.ComponentModel;
using RobotEditor.Enums;

namespace RobotEditor.Classes
{
    [Localizable(false)]
    public sealed class CartesianTypes
    {
        public CartesianEnum ValueCartesianEnum { get; set; }
        public string ValueCartesianString { get; set; }
    }
}