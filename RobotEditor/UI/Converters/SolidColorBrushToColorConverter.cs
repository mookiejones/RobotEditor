using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RobotEditor.UI.Converters;


class SolidColorBrushToColorConverter : SingletonValueConverter<SolidColorBrushToColorConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is SolidColorBrush solidColorBrush ? solidColorBrush.Color : (object)default(Color);

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is Color color ? new SolidColorBrush(color) : default;
}