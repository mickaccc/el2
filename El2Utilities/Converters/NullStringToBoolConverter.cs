using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class NullStringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
                return string.IsNullOrEmpty(s);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
