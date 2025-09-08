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
            if (value == null) return string.Empty;
            DateTime v = (DateTime)value;
            if (parameter == null) parameter = "0";
            _ = int.TryParse(parameter.ToString(), out int par);
   
            int weekNum = culture.Calendar.GetWeekOfYear(v, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            switch(par)
            {
                case 1:
                    return string.Format("{0} KW {1}", v.ToString("dd.MM.yy"), weekNum.ToString());
                case 2:
                    return string.Format("{0}\nKW {1}", v.ToString("dd.MM.yy"), weekNum.ToString());
                case 3:
                    return weekNum.ToString();
                default: return string.Format("KW {0}", weekNum.ToString());
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
