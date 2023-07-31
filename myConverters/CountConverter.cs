using System;
using System.Globalization;
using System.Windows.Data;

namespace Lieferliste_WPF.myConverters
{
    [ValueConversion(typeof(String), typeof(bool))]
    public class CountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String suffix = String.Empty;
            
            if (parameter is String p)
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
