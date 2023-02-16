using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Globalization;
using System.Windows.Data;

namespace Lieferliste_WPF.Converters
{
    [ValueConversion(typeof(String), typeof(int))]
    public class TimeStrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan time = TimeSpan.Parse((String)value);
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
