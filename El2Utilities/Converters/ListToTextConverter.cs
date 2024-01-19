using El2Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace El2Core.Converters
{
    public class ListToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<RessourceCostUnit> l)
            {
                var cost = l.Select(x => x.CostId).ToArray();
                StringBuilder sb = new StringBuilder();
                foreach (var item in cost)
                {
                    sb.Append(string.Format("{0} ", item));
                }

                return sb.ToString();
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
