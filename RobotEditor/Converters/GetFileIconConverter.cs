using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Messaging;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Messages;
using RobotEditor.Utilities;

namespace RobotEditor.Converters
{
    public sealed class GetFileIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result;
            try
            {
                var extension = Path.GetExtension(value.ToString().ToLower());
                if (!String.IsNullOrEmpty(extension))
                {
                    if (extension == ".src")
                    {
                        var bitmapImage = ImageHelper.LoadBitmap(Global.ImgSrc);
                        result = bitmapImage;
                        return result;
                    }
                    if (extension == ".dat")
                    {
                        result = ImageHelper.LoadBitmap(Global.ImgDat);
                        return result;
                    }
                    if (extension == ".sub")
                    {
                        result = ImageHelper.LoadBitmap(Global.ImgSps);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = new ErrorMessage("Convert", ex, MessageType.Error);
                WeakReferenceMessenger.Default.Send<IMessage>(msg);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}