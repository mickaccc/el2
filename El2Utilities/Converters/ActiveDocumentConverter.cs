using Lieferliste_WPF.ViewModels;
using System;
using System.Windows.Data;

namespace El2Utilities.Converters
{


    class ActiveDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DeliveryListViewModel)
                return value;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DeliveryListViewModel)
                return value;

            return Binding.DoNothing;
        }
    }
}
