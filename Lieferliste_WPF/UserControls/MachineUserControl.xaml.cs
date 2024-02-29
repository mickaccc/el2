using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Lieferliste_WPF.Planning
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
                dtx.ProcessesCV.SortDescriptions.Clear();
                dtx.ProcessesCV.SortDescriptions.Add(new SortDescription("Spos", ListSortDirection.Ascending));
            }
        }

        private void HideDetails_Click(object sender, RoutedEventArgs e)
        {
            Planed.SelectedIndex = -1;
            e.Handled = true;
        }

    }
}
