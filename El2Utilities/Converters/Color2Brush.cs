using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace El2Core.Converters
{
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public sealed class Color2Brush : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((SolidColorBrush)value).Color;
        }
    }
}
