using El2Core.Utils;
using System;
using System.Globalization;
using System.Windows.Data;


namespace El2Core.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class CommentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string p = (string)parameter;
            string strVal = (string)value;
            if (p != null && !string.IsNullOrEmpty(strVal))
            {

                string[] val = strVal.Split(';');
                if (p.StartsWith("I"))
                {

                    return val[0];
                }
                else if (p.StartsWith("S") && val.Length > 1)
                {
                    return val[1];
                }
            }
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string info = "[" + UserInfo.User.UserIdent + " - " + DateTime.Now.ToShortDateString() + "]";
                return info + ";" + value;
            }
            return string.Empty;
        }
    }
}
