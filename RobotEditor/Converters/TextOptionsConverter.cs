using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using RobotEditor.Classes;

namespace RobotEditor.Converters
{
    [Localizable(false)]
    public class TextOptionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result;
            if (value is EditorOptions)
            {
                result = (value as EditorOptions);
            }
            else
            {
                Debug.WriteLine("TextOptions Converter Failed {0}", new[]
                {
                    value
                });
                result = Binding.DoNothing;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine("TextOptions Converter Failed ConvertBack {0}", new[]
            {
                value
            });
            return Binding.DoNothing;
        }
    }
}