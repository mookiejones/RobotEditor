using RobotEditor.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace RobotEditor.Converters
{
    public sealed class VariableToFunctionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<IVariable> list = new ObservableCollection<IVariable>();
            if (!(value is ReadOnlyObservableCollection<IVariable> items))
            {
                return Binding.DoNothing;
            }

            System.Collections.Generic.IEnumerable<IVariable> newlist = from item in items
                                                                        where item.Type.ToLower() == "def"
                                                                        select item;


            foreach (IVariable item in newlist)
            {
                list.Add(item);
            }


            return list;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}