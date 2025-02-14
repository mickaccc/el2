using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(DBNull), typeof(string))]
    public class IfNullConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in value)
            {
                if (item != DependencyProperty.UnsetValue) return item;
                
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
