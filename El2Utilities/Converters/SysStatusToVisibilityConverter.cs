using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class SysStatusToVisibilityConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;
            var param = parameter as string;
            if (val != null && param != null)
            {
                return val.Contains(param) ? Visibility.Visible : Visibility.Hidden;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
