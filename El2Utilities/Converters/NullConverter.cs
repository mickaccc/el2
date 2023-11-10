using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion (typeof(DBNull), typeof(int))]
    public class NullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return (int)0;
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int v)
            {
                if (v == 0)
                    return null;
            }
            return (int)value;
        }
    }
}
