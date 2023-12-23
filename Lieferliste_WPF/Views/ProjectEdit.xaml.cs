using El2Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using El2Core.Utils;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            var txtbx = (TextBox)FindName("searchOrder");
            var txt = (e.AddedItems.Count>0) ? e.AddedItems[0] as OrderRb : null;
            if (txt != null)
                txtbx.Text = txt.Aid;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var txtbx = (TextBox)FindName("searchPsp");
            var tree = (TreeView)sender;
            var txt = tree.SelectedItem as TreeNode;
            if(txt != null)
                txtbx.Text = txt.PSP;
        }
    }
}
