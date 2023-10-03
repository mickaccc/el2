﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Utilities.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public sealed class RessourceToKwConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string source = (string)value;
            if(source != String.Empty)
            {
                if (parameter is DateTime date)
                {
                    int weekNum = culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    return String.Format("{0} bis KW{1}' {2}", source, weekNum, date.Year);
                }
            }
            return source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
