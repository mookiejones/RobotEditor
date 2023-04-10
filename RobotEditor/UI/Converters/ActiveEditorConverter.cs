using RobotEditor.Interfaces;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RobotEditor.UI.Converters;


internal sealed class ActiveEditorConverter : SingletonValueConverter<ActiveEditorConverter>
{


    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is IEditorDocument editor ? editor : Binding.DoNothing;

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is IEditorDocument editor ? editor : Binding.DoNothing;
}