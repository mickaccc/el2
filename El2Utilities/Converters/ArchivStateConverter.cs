using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace El2Core.Converters
{
    public class ArchivStateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                Rectangle visualRect = new Rectangle
                {
                    Width = 1,
                    Height = 100,
                    Fill = Brushes.LightGreen,   
                    StrokeThickness = 0.2

                };
                if ((bool)values[0])
                {
                    switch ((int)values[1])
                    {
                        case 1:
  
                            visualRect.Stroke = Brushes.LimeGreen;
                            return new VisualBrush() { Visual = visualRect, TileMode = TileMode.Tile };
                        case 2:

                            visualRect.Stroke = Brushes.SeaGreen;
                            return new VisualBrush() { Visual = visualRect, TileMode = TileMode.Tile };
                        case 3:

                            visualRect.Stroke = Brushes.GreenYellow;
                            return new VisualBrush() { Visual = visualRect, TileMode = TileMode.Tile };
                    }
                    return Brushes.LightGreen;
                }
            }
            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
