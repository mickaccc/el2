using System.Windows.Controls;
using System.Windows.Input;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for ProjectEdit.xaml
    /// </summary>
    public partial class ProjectEdit : UserControl
    {
        public ProjectEdit()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var txtbx = (TextBox)FindName("searchOrder");
            //var txt = (e.AddedItems.Count > 0) ? e.AddedItems[0] as OrderRb : null;
            //if (txt != null)
            //    txtbx.Text = txt.Aid;
        }


        private void SearchPsp_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox psp)
                psp.SelectionStart = psp.Text.Length;
        }

        private void PspTree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //var txtbx = (TextBox)FindName("searchPsp");
            //var tree = (TextBlock)sender;
            //var txt = tree.Text;

            //if (txt != null)
            //    txtbx.Text = txt;

            //e.Handled = true;
        }

        private void ProjectType_Initialized(object sender, System.EventArgs e)
        {
            if (sender is ComboBox s) { s.IsSynchronizedWithCurrentItem = true; }
        }
    }
}
