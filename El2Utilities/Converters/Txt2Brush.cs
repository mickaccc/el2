using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace El2Core.Converters
{
    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public sealed class Txt2Brush : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ColorConverter converter = new ColorConverter();
            Color color = (Color)converter.ConvertFromInvariantString(value.ToString());

            return new SolidColorBrush(color);
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (Color)value;
            ColorConverter converter = new ColorConverter();
            string colorStr = converter.ConvertToInvariantString(color);
            return colorStr;
        }
    }
}
