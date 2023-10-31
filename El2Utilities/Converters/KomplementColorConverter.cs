using System;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class KomplementColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() == typeof(string))
            {
                string val = value.ToString();

                if (val.StartsWith("#") && val.Length == 7)
                {
                    val = val.Replace("#", "");
                    int a = System.Convert.ToInt32(val, 16);
                    a ^= 0xFFFFFF;
                    return "#" + a.ToString("X6");
                }

            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
