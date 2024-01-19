using System.Windows.Controls;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for Archive.xaml
    /// </summary>
    public partial class Archive : UserControl
    {
        public Archive()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "AuftragFarbe"
                || e.PropertyName == "Abgeschlossen"
                || e.PropertyName == "Timestamp"
                || e.PropertyName == "Prio"
                || e.PropertyName == "Fertig"
                || e.PropertyName == "AuftragArt"
                || e.PropertyName == "Ausgebl"
                || e.PropertyName == "Vorgangs"
                || e.PropertyName.EndsWith("Navigation"))

            {
                e.Cancel = true;
            }
            else if (e.PropertyName == "Aid") e.Column.Header = "Auftragsnummer";
            else if (e.PropertyName == "ProId") e.Column.Header = "Projekt";

        }


    }
}
