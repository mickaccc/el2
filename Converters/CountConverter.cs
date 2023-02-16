using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Lieferliste_WPF.Converters
{
    [ValueConversion(typeof(String), typeof(bool))]
    public class CountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String suffix = null;
            string p = parameter as String;
            if (p != null)
            {
                String[] par = p.Split(new char[] { ';' });

                if ((int)value == 1 || par.Length == 1) { suffix = par[0]; } else { suffix = par[1]; }
            }
            return value + " " + suffix;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
