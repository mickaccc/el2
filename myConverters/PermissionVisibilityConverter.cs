using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Lieferliste_WPF.myConverters
{
    [ValueConversion(typeof(String), typeof(Visibility))]
    public class PermissionVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = true;// PermissionsManager.getInstance(MainWindow.currentUser).getUserPermission((String)parameter);
            return b ? Visibility.Visible : Visibility.Hidden;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
