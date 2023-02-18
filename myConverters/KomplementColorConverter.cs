using System;
using System.Windows.Data;

namespace Lieferliste_WPF.myConverters
{
    [ValueConversion(typeof(String), typeof(String))]
    public class KomplementColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() == typeof(String))
            {
                string val = value.ToString();

                if (val.StartsWith("#") && val.Length == 7)
                {
                    val = val.Replace("#", "");
                    Int32 a = System.Convert.ToInt32(val, 16);
                    a ^= 0xFFFFFF;
                    return "#" + a.ToString("X6");
                }

            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
