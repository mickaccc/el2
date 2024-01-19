using System;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace El2Core.Converters
{
    public class RoleToVisibilityConverter : MarkupExtension, IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GenericPrincipal? principal = value as GenericPrincipal;
            bool IsValidUser = false;
            if (principal != null)
            {
                foreach (var _ in from string role in parameter.ToString().Split(';')
                                  where principal.IsInRole(role)
                                  select new { })
                {
                    IsValidUser = true;
                    break;
                }

                return IsValidUser ? Visibility.Visible : Visibility.Collapsed;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
