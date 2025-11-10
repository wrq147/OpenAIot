using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace AirJointUI.Component
{
    public class BackgroundConvert:IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            
            string? result = "Yellow";
            switch (value)
            {
                case 0:
                    //result = "#FAFCFF";
                    result = "Yellow";
                    break;
                case 1:
                    result = "Red";
                    break;
                default:
                    break;
            }
            return result;
            
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value;
            
        }
    }
}
