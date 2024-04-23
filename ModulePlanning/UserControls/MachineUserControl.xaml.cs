using ModulePlanning.Planning;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ModulePlanning.UserControls
{
    /// <summary>
    /// Interaction logic for MachineUserControl.xaml
    /// </summary>
    public partial class MachineUserControl : UserControl
    {

        public MachineUserControl()
        {
            InitializeComponent();

        }

        private void Pl_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ScrollItem")
            {
                Planed.ScrollIntoView((sender as PlanMachine)?.ScrollItem);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //PlanMachine? pl = DataContext as PlanMachine;
            //pl?.Exit();
 

        }


        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dtx = this.DataContext as PlanMachine;
            if (dtx != null)
            {
                var list = dtx.ProcessesCV as ListCollectionView;
                if (list.IsAddingNew) { list.CommitNew(); }
                if (list.IsEditingItem) { list.CommitEdit(); }
                dtx.ProcessesCV.SortDescriptions.Clear();
                dtx.ProcessesCV.SortDescriptions.Add(new SortDescription("SortPos", ListSortDirection.Ascending));
            }
        }

        private void HideDetails_Click(object sender, RoutedEventArgs e)
        {
            var pl = FindName("Planed") as DataGrid;
            pl.SelectedIndex = -1;
            e.Handled = true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var pl = this.DataContext as PlanMachine;
            pl.PropertyChanged += Pl_PropertyChanged;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            shiftOpen.IsChecked = false;
        }
    }
}
