using Azure.Core;
using Lieferliste_WPF.Utilities;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Lieferliste_WPF.myConverters
{
    [ValueConversion(typeof(String), typeof(bool))]
    public class PermissionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            String p = (string)parameter;
            if (p.StartsWith("!"))
            { 
                p = p[1..];
                return !PermissionsProvider.GetUserPermission(p);
            }
            else 
            {
                return PermissionsProvider.GetUserPermission(p);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
