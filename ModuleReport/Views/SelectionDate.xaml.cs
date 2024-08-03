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

namespace ModuleReport.Views
{
    /// <summary>
    /// Interaction logic for SelectionDate.xaml
    /// </summary>
    public partial class SelectionDate : UserControl
    {
        public SelectionDate()
        {
            InitializeComponent();
        }

        private void btnDay_Click(object sender, RoutedEventArgs e)
        {
            var cal = FindName("calendar") as Calendar;
            if (cal != null)
                cal.DisplayMode = CalendarMode.Month;
        }

        private void btnMonth_Click(object sender, RoutedEventArgs e)
        {
            var cal = FindName("calendar") as Calendar;
            if (cal != null)
                cal.DisplayMode = CalendarMode.Year;
        }

        private void btnYear_Click(object sender, RoutedEventArgs e)
        {
            var cal = FindName("calendar") as Calendar;
            if (cal != null)
                cal.DisplayMode = CalendarMode.Decade;
        }
    }
}
