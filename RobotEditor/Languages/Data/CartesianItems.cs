using System.Collections.Generic;
using RobotEditor.Enums;

namespace RobotEditor.Languages.Data
{
    public sealed class CartesianItems : List<CartesianTypes>
    {
        public CartesianItems()
        {
            var item = new CartesianTypes
            {
                ValueCartesianEnum = CartesianEnum.ABB_Quaternion,
                ValueCartesianString = "ABB Quaternion"
            };
            Add(item);
            var item2 = new CartesianTypes
            {
                ValueCartesianEnum = CartesianEnum.Roll_Pitch_Yaw,
                ValueCartesianString = "Roll-Pitch-Yaw"
            };
            Add(item2);
            var item3 = new CartesianTypes
            {
                ValueCartesianEnum = CartesianEnum.Axis_Angle,
                ValueCartesianString = "Axis Angle"
            };
            Add(item3);
            var item4 = new CartesianTypes
            {
                ValueCartesianEnum = CartesianEnum.Kuka_ABC,
                ValueCartesianString = "Kuka ABC"
            };
            Add(item4);
            var item5 = new CartesianTypes
            {
                ValueCartesianEnum = CartesianEnum.Euler_ZYZ,
                ValueCartesianString = "Euler ZYZ"
            };
            Add(item5);
            var item6 = new CartesianTypes
            {
                ValueCartesianEnum = CartesianEnum.Alpha_Beta_Gamma,
                ValueCartesianString = "Alpha-Beta-Gamma"
            };
            Add(item6);
        }
    }
}