using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return "KW " + weekNum.ToString() + "' " + v.Year.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
