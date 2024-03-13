using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace El2Core.Converters
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public sealed class AlertColorConverter : IValueConverter
    {
        public SolidColorBrush? AlertColor { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((bool)value)
            {
                return AlertColor ??= Brushes.Red;
            }
            else
            {
                return Brushes.Transparent;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
