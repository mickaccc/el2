using El2Core.Converters;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for MachineView.xaml
    /// </summary>
    public partial class MachineView : Window
    {
        public MachineView()
        {
            InitializeComponent();
        }


        private void Process_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "CostId" || e.PropertyName == "WorkArId" || e.PropertyName == "resv" || e.PropertyName == "Bullet")
            {
                e.Cancel = true;
            }
            else if (e.PropertyName == "Aid") e.Column.Header = "Auftrg";
            else if (e.PropertyName == "QuantityMiss") e.Column.Header = "offene Menge";
            else if (e.PropertyName == "Text") e.Column.Header = "Vorgang Kurztext";
            else if (e.PropertyName == "Arbid") e.Column.Header = "Arbeitsplatz";
            else if (e.PropertyType == typeof(DateTime?) || e.PropertyType == typeof(DateTime))
            {
                DataGridTextColumn dgtc = e.Column as DataGridTextColumn;
                DateConverter con = new();
                (dgtc.Binding as Binding).Converter = con;
            }

        }
    }
}
