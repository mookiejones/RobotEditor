using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RobotEditor.Converters
{
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        private bool InvertVisibility { get; set; }

        [Localizable(false)]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                bool flag = System.Convert.ToBoolean(value, culture);
                if (InvertVisibility)
                {
                    flag = !flag;
                }
                return flag ? Visibility.Visible : Visibility.Collapsed;
            }
            throw new InvalidOperationException("Converter can only convert to value of type Visibility.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
}