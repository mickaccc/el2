namespace Lieferliste_WPF.Converters
{
  using System;
    using System.Windows.Data;
    using Lieferliste_WPF.ViewModels;

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
