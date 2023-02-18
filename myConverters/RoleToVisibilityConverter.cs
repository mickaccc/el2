using System;
using System.Globalization;
using System.Security.Principal;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Lieferliste_WPF.myConverters
{
    public class RoleToVisibilityConverter : MarkupExtension, IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var principal = value as GenericPrincipal;
            bool IsValidUser = false;
            if (principal != null)
            {
                foreach (String role in parameter.ToString().Split(';'))
                {
                    if (principal.IsInRole(role))
                    {
                        IsValidUser = true;
                        break;
                    }
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
