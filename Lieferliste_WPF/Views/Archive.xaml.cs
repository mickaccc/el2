using El2Core.Models;
using System;
using System.Buffers;
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
