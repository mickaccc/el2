
using El2Utilities.Utils;
using System;
using System.Globalization;
using System.Windows.Data;


namespace El2Utilities.Converters
{
    [ValueConversion(typeof(String), typeof(String))]
    public class CommentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            String p = (string)parameter;
            String strVal = (String)value;
            if (p != null && strVal != String.Empty)
            {
                
                String[] val = strVal.Split(';');
                if (p.StartsWith("I"))
                {

                    return val[0];
                }
                else if (p.StartsWith("S") && val.Length > 1)
                {
                    return val[1];
                }
            }
            return String.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                String info = "[" + AppStatic.User.UserIdent + " - " + DateTime.Now.ToShortDateString() + "]";
                return info + ";" + value;
            }
            return String.Empty;
        }
    }
}
