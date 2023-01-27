using CommunityToolkit.Mvvm.Messaging;
using RobotEditor.Enums;
using RobotEditor.Interfaces;
using RobotEditor.Messages;
using RobotEditor.Utilities;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace RobotEditor.Converters
{
    sealed class GetFileIconConverter : SingletonValueConverter<GetFileIconConverter>
    {
        public override  object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
          
            try
            {
                string extension = Path.GetExtension(value.ToString().ToLower());

                if (string.IsNullOrEmpty(extension))
                    return null;

                switch (extension)
                {
                    case ".src":
                        return ImageHelper.LoadBitmap(Global.ImgSrc);
                    case ".dat":
                        return ImageHelper.LoadBitmap(Global.ImgDat);

                    case ".sub":
                        return ImageHelper.LoadBitmap(Global.ImgSps);
                }

            }
            catch (Exception ex)
            {
                ErrorMessage msg = new ErrorMessage("Convert", ex, MessageType.Error);
                _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}