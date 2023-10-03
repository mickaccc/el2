using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace El2Utilities.Converters
{
    public class NullVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultInvisibility = parameter as string;
            Visibility invisibility = defaultInvisibility != null ?
                (Visibility)Enum.Parse(typeof(Visibility), defaultInvisibility)
                : Visibility.Collapsed;
            return value != null ? invisibility : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
