﻿using System;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class MinuteSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var i = (int)value / 5;
            return (double)i;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
