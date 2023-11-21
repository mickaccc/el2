using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            try
            {
                var v = value;
                if (value == null) return null;
                DateTime date = (DateTime)value;
                return date.ToString("ddd, dd.MM.yyyy");
            }
            catch (Exception)
            {

                return null;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? strValue = value as string;
            if (DateTime.TryParse(strValue, out DateTime resultDateTime))
            {
                return resultDateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
