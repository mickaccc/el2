using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Lieferliste_WPF.myConverters
{
    [ValueConversion(typeof(String), typeof(Color))]
    public sealed class Txt2Color : IValueConverter
    {
        object IValueConverter.Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {

            try
            {
                
                ColorConverter converter = new();
                Color? color;
                if (value == null)
                {
                    return (Color)converter.ConvertFromInvariantString("#FFFFFFFF");
                }
                else
                {
                    color = (Color)converter.ConvertFromInvariantString(value.ToString());
                }
                return color;
            }
            catch (FormatException)
            {
                return null;
            }
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {   
            Color color = (Color)value;
            ColorConverter converter = new();
            String? colorStr = converter.ConvertToInvariantString(color);
            return colorStr;
        }
    }
}
