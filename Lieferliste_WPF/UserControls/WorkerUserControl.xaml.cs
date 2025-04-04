﻿using Lieferliste_WPF.Planning;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// Interaction logic for MachineUserControl.xaml
    /// </summary>
    public partial class WorkerUserControl : UserControl
    {

        public WorkerUserControl()
        {
            InitializeComponent();
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
            var dtx = DataContext as PlanWorker;
            if (dtx != null)
            {
                dtx.ProcessesCV.SortDescriptions.Clear();
                dtx.ProcessesCV.SortDescriptions.Add(new SortDescription("SortPos", ListSortDirection.Ascending));
            }
        }

    }
}
