using CommunityToolkit.Mvvm.ComponentModel;
using RobotEditor.Controls.AngleConverter;
using RobotEditor.Controls.AngleConverter.Classes;
using RobotEditor.Enums;
using RobotEditor.ViewModel;
using System;

namespace RobotEditor.Tools.AngleConverter;

public sealed partial class AngleConvertorViewModel : ToolViewModel
{
    public const string ToolContentId = "AngleConverterTool";

    private bool _isConverting;

    [ObservableProperty]
    private string _matrix = string.Empty;

    private RotationMatrix3D _rotationMatrix = RotationMatrix3D.Identity();

    public AngleConvertorViewModel()
        : base("Angle Converter")
    {
        InputItems.ItemsChanged += (s, e) => Convert();
        OutputItems.ItemsChanged += (s, e) => Convert();
        ContentId = "AngleConverterTool";
        DefaultPane = DefaultToolPane.Right;
    }

    public static CartesianEnum CartesianType { get; set; }

    [ObservableProperty]
    private ValueBoxViewModel inputItems = new();

    [ObservableProperty]
    private ValueBoxViewModel _outputItems = new() { IsReadOnly = true };

    public string Error => null;


    public void Convert()
    {
        if (!(InputItems == null | OutputItems == null))
        {
            if (!_isConverting)
            {
                _isConverting = true;
                Vector3D result = new();
                double num = 0.0;
                Quaternion quaternion = new();
                switch (InputItems.SelectedItem)
                {
                    case CartesianEnum.ABB_Quaternion:
                        quaternion = new Quaternion(InputItems.V1, InputItems.V2, InputItems.V3, InputItems.V4);
                        break;
                    case CartesianEnum.Roll_Pitch_Yaw:
                        _rotationMatrix = RotationMatrix3D.FromRPY(InputItems.V1, InputItems.V2, InputItems.V3);
                        break;
                    case CartesianEnum.Axis_Angle:
                        _rotationMatrix =
                            RotationMatrix3D.RotateAroundVector(
                                new Vector3D(InputItems.V1, InputItems.V2, InputItems.V3), InputItems.V4);
                        break;
                    case CartesianEnum.Kuka_ABC:
                        _rotationMatrix = RotationMatrix3D.FromABC(InputItems.V1, InputItems.V2, InputItems.V3);
                        break;
                    case CartesianEnum.Euler_ZYZ:
                        _rotationMatrix = RotationMatrix3D.FromEulerZYZ(InputItems.V1, InputItems.V2, InputItems.V3);
                        break;
                }
                switch (OutputItems.SelectedItem)
                {
                    case CartesianEnum.ABB_Quaternion:
                        quaternion = (Quaternion)_rotationMatrix;
                        result = quaternion.Vector;
                        num = quaternion.Scalar;

                        break;
                    case CartesianEnum.Roll_Pitch_Yaw:
                        result = _rotationMatrix.RPY;
                        break;
                    case CartesianEnum.Axis_Angle:
                        result = _rotationMatrix.RotationAxis();
                        num = _rotationMatrix.RotationAngle();
                        break;
                    case CartesianEnum.Kuka_ABC:
                        result = _rotationMatrix.ABC;
                        break;
                    case CartesianEnum.Euler_ZYZ:
                        result = _rotationMatrix.EulerZYZ;
                        break;
                    case CartesianEnum.Alpha_Beta_Gamma:
                        result = _rotationMatrix.ABG;
                        break;
                }
                string text = quaternion.ToString();

                //                    var text = quaternion.ToString("F3");
                if (Matrix != null && Matrix != text)
                {
                    Matrix = text;
                }
                WriteValues(result, 0.0, false);
                if (OutputItems.SelectedItem == CartesianEnum.ABB_Quaternion)
                {
                    WriteValues(result, num, true);
                }
                if (OutputItems.SelectedItem == CartesianEnum.Axis_Angle)
                {
                    OutputItems.V4 = num;
                }
                _isConverting = false;
            }
        }
    }

    private void WriteValues(Vector3D result, double scalar, bool isScalar)
    {
        switch (isScalar)
        {
            case false:
                OutputItems.V1 = result.X;
                OutputItems.V2 = result.Y;
                OutputItems.V3 = result.Z;
                break;
            case true:
                OutputItems.V1 = scalar;
                OutputItems.V2 = result.X;
                OutputItems.V3 = result.Y;
                OutputItems.V4 = result.Z;
                break;
        }
    }
}