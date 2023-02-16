using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Drawing;
using System.Windows.Data;

namespace Lieferliste_WPF.Converters
{
    public class ColorToBrushKomplementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                //return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                return "#FF00FF";
            if (value is Color)

                //return new SolidColorBrush((Color)value);
                return "#FF00FF";
            if (value is string)
                //return new SolidColorBrush(Parse((string)value));
                return "#FF00FF";

            throw new NotSupportedException("ColorToBurshConverter only supports converting from Color and String");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }


    }
}
