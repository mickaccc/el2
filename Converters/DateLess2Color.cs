using System;
using System.Globalization;
using System.Windows.Data;

namespace Lieferliste_WPF.Converters
{
    [ValueConversion(typeof(DateTime), typeof(String))]
    class DateLess2Color :IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            System.Drawing.Color c;
            if (date < DateTime.Now.Date)
            {
                c = Properties.Settings.Default.outOfDate;
            }
            else
            {
                c = Properties.Settings.Default.inDate;
            }
            return "#" + c.ToArgb().ToString("X6");
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
