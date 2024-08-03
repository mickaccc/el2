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
    /// Interaction logic for MaterialResultList.xaml
    /// </summary>
    public partial class MaterialResultList : UserControl
    {
        public MaterialResultList()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if(e.PropertyName == "IsActive" ||
                e.PropertyName == "Responses" ||
                e.PropertyName == "InventNos")
            {
                e.Cancel = true;
            }
        }

        private void DG_LayoutUpdated(object sender, EventArgs e)
        {
            Thickness t = lblTotal.Margin;
            t.Left = (DGSalesINvoice.Columns[0].ActualWidth + 7);
            lblTotal.Margin = t;
            lblTotal.Width = DGSalesINvoice.Columns[1].ActualWidth;

            lblTotalSalesInvoiceAmount.Width = DGSalesINvoice.Columns[2].ActualWidth;
        }
    }
}
