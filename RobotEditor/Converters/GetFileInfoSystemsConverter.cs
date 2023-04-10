using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace RobotEditor.Converters;

 sealed class GetFileSystemInfosConverter : SingletonValueConverter<GetFileSystemInfosConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
         
        try
        {
            if (value is DirectoryInfo directoryInfo)
                return directoryInfo.GetFileSystemInfos();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return null;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}