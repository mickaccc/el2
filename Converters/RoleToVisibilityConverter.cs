using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Security.Principal;
using System.Windows.Markup;
using System.Windows.Data;

namespace Lieferliste_WPF.Converters
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
