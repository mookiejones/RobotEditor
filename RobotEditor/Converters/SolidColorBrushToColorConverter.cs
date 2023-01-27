using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
namespace RobotEditor.Converters
{

	public class SolidColorBrushToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
            if (value is SolidColorBrush solidColorBrush)
            {
                return solidColorBrush.Color;
            }
            return default(Color);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				Color color = (Color)value;
				return new SolidColorBrush(color);
			}
			return null;
		}
	}

}