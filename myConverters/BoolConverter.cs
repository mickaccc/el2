using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Lieferliste_WPF.myConverters
{
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public sealed class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool v = (bool)value;
            return !v;
        }
        public object ConvertBack(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotSupportedException();
        }
    }
}
