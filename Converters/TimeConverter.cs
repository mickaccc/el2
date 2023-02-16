using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Globalization;
using System.Windows.Data;

namespace Lieferliste_WPF.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(String))]
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan time = (TimeSpan)value;
            return time.ToString(@"hh\:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            TimeSpan resultTimeSpan;
            if (TimeSpan.TryParse(strValue, out resultTimeSpan))
            {
                return resultTimeSpan;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
