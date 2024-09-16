using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModuleShift.Views
{
    /// <summary>
    /// Interaction logic for ShiftPlanEdit.xaml
    /// </summary>
    public partial class ShiftPlanEdit : UserControl
    {
        public ShiftPlanEdit()
        {
            InitializeComponent();
        }

  

        private void cmbShiftPlan_GotFocus(object sender, RoutedEventArgs e)
        {
            var cmb = sender as ComboBox;
            var lb = FindName("lvShiftWeek") as ListBox;
            var it = cmb.GetVisualAncestor<ListBoxItem>();
            lb.SelectedItem = it.Content;          
        }
    }
}
