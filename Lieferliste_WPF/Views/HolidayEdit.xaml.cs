using System.Windows.Controls;

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
