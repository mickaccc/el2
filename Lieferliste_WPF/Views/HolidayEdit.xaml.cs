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

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for HolidayEdit.xaml
    /// </summary>
    public partial class HolidayEdit : UserControl
    {
        public HolidayEdit()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Locale"
                || e.PropertyName == "Type")
                    e.Cancel = true;
            
        }
        private void FixDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Locale"
                || e.PropertyName == "DayDistance")
            {
                e.Cancel = true;
            }
            else if (e.PropertyName == "Day") e.Column.Header = "Tag";
            else if (e.PropertyName == "Month") e.Column.Header = "Monat";

        }
        private void FlexDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Locale"
                || e.PropertyName == "Day"
                || e.PropertyName == "Month")
            {
                e.Cancel = true;
            }
            else if (e.PropertyName == "DayDistance") e.Column.Header = "Tagesdifferenz zu Ostern";

        }
    }
}
