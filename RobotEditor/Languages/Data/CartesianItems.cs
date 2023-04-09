using RobotEditor.Enums;
using System.Collections.Generic;

namespace RobotEditor.Languages.Data
{
    public sealed class CartesianItems : List<CartesianTypes>
    {
        public CartesianItems()
        {
            CartesianTypes item = new()
            {
                ValueCartesianEnum = CartesianEnum.ABB_Quaternion,
                ValueCartesianString = "ABB Quaternion"
            };
            Add(item);
            CartesianTypes item2 = new()
            {
                ValueCartesianEnum = CartesianEnum.Roll_Pitch_Yaw,
                ValueCartesianString = "Roll-Pitch-Yaw"
            };
            Add(item2);
            CartesianTypes item3 = new()
            {
                ValueCartesianEnum = CartesianEnum.Axis_Angle,
                ValueCartesianString = "Axis Angle"
            };
            Add(item3);
            CartesianTypes item4 = new()
            {
                ValueCartesianEnum = CartesianEnum.Kuka_ABC,
                ValueCartesianString = "Kuka ABC"
            };
            Add(item4);
            CartesianTypes item5 = new()
            {
                ValueCartesianEnum = CartesianEnum.Euler_ZYZ,
                ValueCartesianString = "Euler ZYZ"
            };
            Add(item5);
            CartesianTypes item6 = new()
            {
                ValueCartesianEnum = CartesianEnum.Alpha_Beta_Gamma,
                ValueCartesianString = "Alpha-Beta-Gamma"
            };
            Add(item6);
        }
    }
}