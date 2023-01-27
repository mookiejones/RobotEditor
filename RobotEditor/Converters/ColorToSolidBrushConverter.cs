using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
namespace RobotEditor.Converters
{

  
    //
    // Summary:
    //     Converts a given System.Windows.Media.Color into a System.Windows.Media.SolidColorBrush.
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]

    class ColorToSolidColorBrushConverter : SingletonValueConverter<ColorToSolidColorBrushConverter>
    {
 
        //
        // Summary:
        //     Gets or Sets the brush which will be used if the conversion fails.
        public SolidColorBrush FallbackBrush { get; set; }

        //
        // Summary:
        //     Gets or Sets the color which will be used if the conversion fails.
        public Color? FallbackColor { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                SolidColorBrush solidColorBrush = new SolidColorBrush(color);
                solidColorBrush.Freeze();
                return solidColorBrush;
            }

            return FallbackBrush;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (value is SolidColorBrush solidColorBrush) ? new Color?(solidColorBrush.Color) : FallbackColor;
    }
}
