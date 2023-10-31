using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    class FillZerosConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = 0;
            var t = value.GetType();
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
