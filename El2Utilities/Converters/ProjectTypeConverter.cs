using El2Core.Constants;
using El2Core.Utils;
using System;
using System.Globalization;
using System.Windows.Data;

namespace El2Core.Converters
{
    public class ProjectTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ( value is ProjectTypes.ProjectType projectType ) { return projectType.Description(); }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { 
            return value;
        }
    }
}
