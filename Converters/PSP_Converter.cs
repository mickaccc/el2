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
    public sealed class PSP_Converter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            String strVal = (String)value;
            Regex regex = new Regex("(DS)([0-9]{6})([0-9]{2})*");
            Match match = regex.Match(strVal);
            if (match.Success)
            {
                String retVal;
                retVal = match.Groups[1] + "-" + match.Groups[2];
                foreach (System.Text.RegularExpressions.Capture m in match.Groups[3].Captures)
                {
                    retVal += "-" + m.Value;
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
