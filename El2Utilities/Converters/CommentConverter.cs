﻿
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

            string strVal = (string)value;
            if (!string.IsNullOrEmpty(strVal))
            {

                string[] val = strVal.Split((char)29);
                if ((bool)parameter)
                {
                    return val[0];
                }
                else
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
                return info + (char)29 + value;
            }
            return string.Empty;
        }
    }
}
