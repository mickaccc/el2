using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Lieferliste_WPF.Converters
{
    [ValueConversion(typeof(int), typeof(String))]
    class FillZerosConverter:IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = 0;
            var t = value.GetType();
            if (value.GetType()==typeof(String))
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
