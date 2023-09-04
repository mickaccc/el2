using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace Lieferliste_WPF.myConverters
{
    [ValueConversion(typeof(DateTime), typeof(SolidColorBrush))]
    public sealed class DateLostToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DateTime d)
            {
                if (d < DateTime.Now)
                {
                    return new BrushConverter().ConvertFromString(Properties.Settings.Default.outOfDate.Name);
                }              
            }
            return new BrushConverter().ConvertFromString(Properties.Settings.Default.inDate.Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
