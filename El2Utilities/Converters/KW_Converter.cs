using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class KW_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime v = (DateTime)value;
            int weekNum = culture.Calendar.GetWeekOfYear(v, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return string.Format("{0}\nKW {1}", v.ToString("dd.MM.yy"),  weekNum.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
