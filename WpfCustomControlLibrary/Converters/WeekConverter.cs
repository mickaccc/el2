using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfCustomControlLibrary.Converters
{
    public class WeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {

                if (int.TryParse((string?)parameter, out int row))
                {
                    var date = new DateOnly(dt.Year, dt.Month, 1);
             
                    return culture.Calendar.GetWeekOfYear(date.AddDays(row*7).ToDateTime(TimeOnly.MinValue),
                        CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString();

                }           
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
