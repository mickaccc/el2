using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(Single), typeof(Single))]
    public class PlusConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            Single v = (value[0] == null) ? 0 : (Single)value[0];
            Single p = (value[1] == null) ? 0 : (Single)value[1];

            return string.Format("{0:F2}h", (v + p) / 60);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
