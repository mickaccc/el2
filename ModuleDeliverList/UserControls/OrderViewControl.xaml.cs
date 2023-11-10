using System.Windows.Controls;
namespace ModuleDeliverList.UserControls
{
    /// <summary>
    /// Interaction logic for OrderView.xaml
    /// </summary>
    public partial class OrderViewControl : UserControl
    {


        public OrderViewControl()
        {
            
            InitializeComponent();
            this.listBox1.Items.SortDescriptions.Add(
                new System.ComponentModel.SortDescription("Vnr",System.ComponentModel.ListSortDirection.Ascending) );
            this.listBox1.Items.Refresh();
        }


    }

}
