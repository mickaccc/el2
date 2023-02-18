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
            //return PermissionsManager.getInstance(MainWindow.currentUser).getUserPermission((String)parameter);
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
