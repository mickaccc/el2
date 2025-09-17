using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using vhCalendar;
using WpfCustomControlLibrary;

namespace ModuleProducts.Views
{
    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class Products : UserControl
    {
        public Products()
        {
            InitializeComponent();
        }


        private void Single_Click(object sender, RoutedEventArgs e)
        {
            CLD.SelectionMode = SelectionType.Single;
            var mnitem = (MenuItem)sender;
            mnitem.IsEnabled = false;

            Multiply.IsEnabled = true;
            Week.IsEnabled = true;
            Range.IsEnabled = true;
        }

        private void Multiply_Click(object sender, RoutedEventArgs e)
        {
            CLD.SelectionMode = SelectionType.Multiple;
            var mnitem = (MenuItem)sender;
            mnitem.IsEnabled = false;
            Single.IsEnabled = true;
            Week.IsEnabled = true;
            Range.IsEnabled = true;
        }

        private void Week_Click(object sender, RoutedEventArgs e)
        {
            CLD.SelectionMode = SelectionType.Week;
            var mnitem = (MenuItem)sender;
            mnitem.IsEnabled = false;
            Single.IsEnabled = true;
            Multiply.IsEnabled = true;
            Range.IsEnabled = true;
        }

        private void Range_Click(object sender, RoutedEventArgs e)
        {
            //CLD.SelectionMode = SelectionType.Range;
            var mnitem = (MenuItem)sender;
            mnitem.IsEnabled = false;
            Single.IsEnabled = true;
            Multiply.IsEnabled = true;
            Week.IsEnabled = true;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            CLD.SelectedDates.Clear();
        }
    }
}
