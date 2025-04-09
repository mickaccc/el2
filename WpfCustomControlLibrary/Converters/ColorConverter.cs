using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfCustomControlLibrary.Converters
{
    internal class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;
            if (b)
            {
                return Brushes.LightBlue;
            }
            else { return Brushes.Gray; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
