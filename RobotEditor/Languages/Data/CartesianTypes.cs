using System.ComponentModel;
using RobotEditor.Enums;

namespace RobotEditor.Languages.Data
{
    [Localizable(false)]
    public sealed class CartesianTypes
    {
        public CartesianEnum ValueCartesianEnum { get; set; }
        public string ValueCartesianString { get; set; }
    }
}