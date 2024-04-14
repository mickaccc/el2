using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Lieferliste_WPF.Planning
{
    /// <summary>
    /// Interaction logic for MachineUserControl.xaml
    /// </summary>
    public partial class WorkerUserControl : UserControl
    {

        public WorkerUserControl()
        {
            InitializeComponent();
            //Planed.SelectedIndex = -1;
        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void HideDetails_Click(object sender, RoutedEventArgs e)
        {
            Planed.SelectedIndex = -1;
            e.Handled = true;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dtx = this.DataContext as PlanWorker;
            if (dtx != null)
            {
                dtx.ProcessesCV.SortDescriptions.Clear();
                dtx.ProcessesCV.SortDescriptions.Add(new SortDescription("Spos", ListSortDirection.Ascending));
            }
        }
    }
}
