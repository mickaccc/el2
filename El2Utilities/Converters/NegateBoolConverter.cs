using System;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public sealed class NegateBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool v = (bool)value;
            return !v;
        }
        public object ConvertBack(object value, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
