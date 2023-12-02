using Microsoft.IdentityModel.Tokens;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace El2Core.Converters
{
    [ValueConversion(typeof(string[]), typeof(SolidColorBrush))]
    public class TextCompareToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values == null || values.Length != 2) return false;
            string v1 = (string)values[0];
            string v2 = (string)values[1];
            if (!v2.IsNullOrEmpty() && v1.Contains(v2, StringComparison.CurrentCultureIgnoreCase))
            {
                return Brushes.Khaki;
            }
            else return Brushes.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
