using El2Core.Utils;
using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class PermissionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string p = (string)parameter;
            if (p.StartsWith("!"))
            {
                p = p[1..];

                return !PermissionsProvider.GetInstance().GetUserPermission(p);
            }
            else
            {
                return PermissionsProvider.GetInstance().GetUserPermission(p);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
