using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Core.Converters
{
    public class DoubleToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int hour, minute;
            double m;
            if (value is double dbl)
            {
                hour = (int)dbl;
                m = dbl - Math.Truncate(dbl);
                m = Math.Round(m*60, 2);
                minute = (int)m;

                return string.Format("{0}:{1}", hour, minute.ToString("D2"));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
