using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Media;

namespace Lieferliste_WPF.Converters
{
    [ValueConversion(typeof(System.Drawing.Color), typeof(System.Windows.Media.Color))]
    class Color2MediaBrush :IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Drawing.Color col = (System.Drawing.Color)value;
            return System.Windows.Media.Color.FromArgb(col.A, col.R, col.G,col.B);
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((SolidColorBrush)value).Color;
        }
    }
}
