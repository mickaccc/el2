using Lieferliste_WPF.Planning;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lieferliste_WPF.myConverters
{
    [ValueConversion(typeof(String), typeof(int))]
    class ToIntConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int ret=0;
            if(value.GetType() == typeof(Byte))
            {
                ret = (int)((Byte)value);
            }
            return ret;

           
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Byte)((int)value);
        }
    }
}
