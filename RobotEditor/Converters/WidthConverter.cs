using RobotEditor.Enums;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace RobotEditor.Converters
{
    [Localizable(false)]
    public sealed class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result;
            if (value is GridView)
            {
                GridView gridView = value as GridView;
                result = gridView;
            }
            else
            {
                if (value is CartesianEnum)
                {
                    switch ((CartesianEnum)value)
                    {
                        case CartesianEnum.ABB_Quaternion:
                        case CartesianEnum.Axis_Angle:
                            result = "25*";
                            return result;
                    }
                    result = "33*";
                }
                else
                {
                    result = null;
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}