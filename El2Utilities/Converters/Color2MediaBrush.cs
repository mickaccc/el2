using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace El2Core.Converters
{
    [ValueConversion(typeof(System.Drawing.Color), typeof(Color))]
    internal class Color2MediaBrush : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Drawing.Color col = (System.Drawing.Color)value;
            return Color.FromArgb(col.A, col.R, col.G, col.B);
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((SolidColorBrush)value).Color;
        }
    }
}
