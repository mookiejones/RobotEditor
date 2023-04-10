using RobotEditor.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace RobotEditor.Converters;

[ValueConversion(typeof(ReadOnlyObservableCollection<IVariable>), typeof(ObservableCollection<IVariable>))]
sealed class VariableToFunctionConverter : SingletonValueConverter<VariableToFunctionConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not ReadOnlyObservableCollection<IVariable> items)
        {
            return Binding.DoNothing;
        }


        return   items.Where(o => o.Type.ToLower() == "def")
                     .ToList();
      
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}