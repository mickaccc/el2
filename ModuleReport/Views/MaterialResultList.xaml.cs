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
    }
}
