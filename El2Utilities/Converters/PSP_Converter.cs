using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace El2Utilities.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public sealed partial class PSP_Converter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var strVal = (string)value;
            Regex regex = new Regex("(DS)([0-9]{6})([0-9]{2})*");
            var match = regex.Match(strVal);
            if (match.Success)
            {
                string retVal;
                retVal = match.Groups[1] + "-" + match.Groups[2];
                foreach (var m in match.Groups[3].Captures.Cast<Capture>())
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
