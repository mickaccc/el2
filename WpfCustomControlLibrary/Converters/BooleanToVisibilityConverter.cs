using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfCustomControlLibrary.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        // If the value is 'true' it will be interpreated as 'Visible' else 'Collapsed'  
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
