﻿using System;
using System.Windows.Data;

namespace El2Core.Converters
{

    public class NegatingConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double v)
            {
                return -v;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double v)
            {
                return +v;
            }
            return value;
        }
    }

}
