using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using El2Core.Constants;

namespace El2Core.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        { 
            switch(Properties.Settings.Default.EmployTimeFormat)
            {
                case (int)Formats.TimeFormat.hour_minute:
                    return TimeSpan.FromHours((double)value).ToString(@"hh\:mm");
                case (int)Formats.TimeFormat.minute:
                    return string.Format("{0} Min.", TimeSpan.FromHours((double)value).TotalMinutes);
                case (int)Formats.TimeFormat.hour:
                    return string.Format("{0} Std.", TimeSpan.FromHours((double)value).TotalHours); ;
                default: return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
