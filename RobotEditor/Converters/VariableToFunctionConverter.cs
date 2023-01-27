using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using RobotEditor.Interfaces;

namespace RobotEditor.Converters
{
    public sealed class VariableToFunctionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = new ObservableCollection<IVariable>();
            if (!(value is ReadOnlyObservableCollection<IVariable> items))
                return Binding.DoNothing;
            var newlist = from item in items
                where item.Type.ToLower() == "def"
                select item;

           
            foreach (var item in newlist)
            {
                list.Add(item);
            }


            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}