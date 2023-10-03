using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace El2Utilities.Converters
{
    [ValueConversion(typeof(string), typeof(int))]
    public class TimeStrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan time = TimeSpan.Parse((string)value);
            return time.TotalMinutes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(int))
            {
                return TimeSpan.FromMinutes((double)value).ToString(@"hh\:mm");
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
