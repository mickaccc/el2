﻿using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Windows.Data;

namespace Lieferliste_WPF.myConverters
{
    [ValueConversion(typeof(string), typeof(string))]
    public sealed class RessourceToKwConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!((string)value).IsNullOrEmpty())
            {
                if (parameter is DateTime date)
                {
                    int weekNum = culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    return String.Format("{0} bis KW{1}' {2}", value, weekNum, date.Year);
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
