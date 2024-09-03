﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Core.Converters
{
    [ValueConversion(typeof(Single), typeof(Single))]
    public class PlusConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            Single d = 0;
            foreach (var item in value)
            {
                if (item != null)
                    if (item is Single s)
                    {
                        d += s;
                    }
            }

            return string.Format("{0:F2}h", d / 60);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
