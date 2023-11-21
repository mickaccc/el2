using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    internal class FillZerosConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var t = value.GetType();
            int intValue;
            if (value.GetType() == typeof(string))
            {
                if (!int.TryParse((string)value, out intValue)) return value;
            }

            else { intValue = Convert.ToInt16(value); }

            string format = (string)parameter;
            return intValue.ToString(format);
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
