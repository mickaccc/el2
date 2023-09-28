using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Utilities.Converters
{
    [ValueConversion(typeof(string), typeof(int))]
    public class ToIntConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int ret = 0;
            if (value.GetType() == typeof(byte))
            {
                ret = (byte)value;
            }
            return ret;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (byte)(int)value;
        }
    }
}
