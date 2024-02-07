using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace El2Core.Converters
{
    [ValueConversion(typeof(DateTime), typeof(bool))]
    public sealed class LowerThanDate : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime d)
            {
                return d < DateTime.Now;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
