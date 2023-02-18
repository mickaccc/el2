using System;
using System.Globalization;
using System.Windows.Data;

namespace Lieferliste_WPF.myConverters
{
    [ValueConversion(typeof(int), typeof(String))]
    class FillZerosConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = 0;
            var t = value.GetType();
            if (value.GetType() == typeof(String))
            {
                if (!int.TryParse((String)value, out intValue)) return value;
            }

            else { intValue = Convert.ToInt16(value); }

            String format = (String)parameter;
            return intValue.ToString(format);
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
