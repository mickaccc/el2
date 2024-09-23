using Microsoft.IdentityModel.Tokens;
using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class NullZeroToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is Single s)
            {
               return s != 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
