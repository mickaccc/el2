using El2Core.Utils;
using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(string[]), typeof(bool))]
    public class RelativePermissionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2)
            {
                var v1 = values[0] as string;
                if (int.TryParse(values[1]?.ToString(), out int v2))
                    if (v1 != null)
                        return PermissionsProvider.GetInstance().GetRelativeUserPermission(v1, v2);               
            }

            return false;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
