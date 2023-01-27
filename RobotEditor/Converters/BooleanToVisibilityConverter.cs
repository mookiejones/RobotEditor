using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RobotEditor.Converters
{
    [ValueConversion(typeof(bool),typeof(Visibility))]
     sealed class BooleanToVisibilityConverter : SingletonValueConverter<BooleanToVisibilityConverter>
    {
        private bool InvertVisibility { get; set; }

        [Localizable(false)]
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                bool flag = System.Convert.ToBoolean(value, culture);
                if (InvertVisibility)                
                    flag = !flag;


                return flag ? Visibility.Visible : Visibility.Collapsed;
            }
            throw new InvalidOperationException("Converter can only convert to value of type Visibility.");
        }

        public override  object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (Visibility)value == Visibility.Visible;
    }
}