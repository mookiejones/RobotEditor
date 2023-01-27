using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RobotEditor.Converters
{
    internal abstract class SingletonValueConverter<T> : IValueConverter
        where T : class, new()
    {
        
        private static T defaultInstance;
        public static T DefaultInstance => defaultInstance??(defaultInstance= new T());

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}
