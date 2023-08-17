using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace Lieferliste_WPF.myConverters
{
    [ValueConversion(typeof(string), typeof(string))]
    public sealed partial class TTNR_Converter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var strVal = (value as string).Trim();

                var regex = MyRegex();
                var match = regex.Match(strVal);
                if (match.Success)
                {
                    
                    var retVal = match.Groups[1] + "." + match.Groups[2] + "." + match.Groups[3];
                    if (strVal.Length > 10)
                    {
                        retVal += "-" + match.Groups[4];
                    }
                    return retVal;
                }
            }
            return value;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        [GeneratedRegex("([a-zA-Z0-9]{4})([a-zA-Z0-9]{3})([a-zA-Z0-9]{3})([a-zA-Z0-9]*)")]
        private static partial Regex MyRegex();
    }
}
