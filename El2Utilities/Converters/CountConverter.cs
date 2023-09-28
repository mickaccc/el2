using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Utilities.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class CountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string suffix = string.Empty;

            if (parameter is string p)
            {
                string[] par = p.Split(new char[] { ';' });

                if ((int)value == 1 || par.Length == 1) { suffix = par[0]; } else { suffix = par[1]; }
            }
            return value + " " + suffix;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
