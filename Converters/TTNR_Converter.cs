using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Lieferliste_WPF.Converters
{
    [ValueConversion(typeof(String), typeof(String))]
    public sealed class TTNR_Converter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            String strVal = (value as String).Trim();
            Regex regex = new Regex("([a-zA-Z0-9]{4})([a-zA-Z0-9]{3})([a-zA-Z0-9]{3})([a-zA-Z0-9]*)");
            Match match = regex.Match(strVal);
            if (match.Success)
            {
                String retVal;
                retVal = match.Groups[1] + "." + match.Groups[2] + "." + match.Groups[3];
                if (strVal.Length > 10)
                {
                    retVal += "-" + match.Groups[4];
                }
                return retVal;
            }

            return value;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
