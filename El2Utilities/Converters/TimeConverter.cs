using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace El2Utilities.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
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
