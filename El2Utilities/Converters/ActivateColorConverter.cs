using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace El2Core.Converters
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public sealed class ActivateColorConverter : IValueConverter
    {
        public SolidColorBrush ActiveColor { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((bool)value)
            {
                return ActiveColor;
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
