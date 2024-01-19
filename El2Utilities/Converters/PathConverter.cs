using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Core.Converters
{
    public class PathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Console.WriteLine("Value:" + value);
            return "/Images/" + value + ".png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
