using El2Core.Models;
using Lieferliste_WPF.ViewModels;
using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for MachinePlan.xaml
    /// </summary>
    public partial class MachinePlan : UserControl
    {
        [SupportedOSPlatform("windows7.0")]
        public MachinePlan()
        {
            InitializeComponent();

        }

        //private void UnPlanedMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        dynamic vrg = UnPlaned.SelectedItem as dynamic;
        //        if (vrg != null)
        //        {
        //            String vid = vrg.v.VorgangId;

        //            DataObject data = new DataObject(vid);
        //            DragDrop.DoDragDrop(UnPlaned, data, DragDropEffects.Move);
        //            e.Handled = true;
        //        }
        //    }
        //}

        //private void UnPlaned_DragOver(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(DataFormats.StringFormat))
        //        e.Effects = DragDropEffects.Move;
        //    else
        //        e.Effects = DragDropEffects.None;
        //    e.Handled = true;
        //}

        //private void UnPlaned_Drop(object sender, DragEventArgs e)
        //{
        //    string d = (string)e.Data.GetData(DataFormats.StringFormat);
        //    (Main.DataContext as MachinePlanViewModel).DragProcess(d,0);
        //    e.Handled= true;
        //}

        private void MPL_Unloaded(object sender, RoutedEventArgs e)
        {
            (Main.DataContext as MachinePlanViewModel).Exit();
        }

        //private void UnPlaned_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        //{
        //    if (e.Effects == DragDropEffects.Move)
        //    {
        //        e.UseDefaultCursors = false;
        //        Mouse.SetCursor(Cursors.Hand);
        //    }
        //    else
        //        e.UseDefaultCursors = true;

        //    e.Handled = true;
        //}



        private void UnPlanedCVS_Filter(object sender, FilterEventArgs e)
        {
            if ((e.Item as Vorgang).Aid.Contains(searchTextBox.Text))
                e.Accepted = true;
            else e.Accepted = false;
        }

        private void UnPlaned_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
