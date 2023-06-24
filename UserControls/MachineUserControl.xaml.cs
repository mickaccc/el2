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
using WpfCustomControlLibrary;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Lieferliste_WPF.Planning;
using System.DirectoryServices.Protocols;
using Lieferliste_WPF.Commands;
using Lieferliste_WPF.View;

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// Interaction logic for MachineUserControl.xaml
    /// </summary>
    public partial class MachineUserControl : UserControl
    {
        
        public MachineUserControl()
        {           
            InitializeComponent();
            Planed.SelectedIndex = -1;
        }



        private void Planed_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                dynamic vrg = Planed.SelectedItem as dynamic;
                if (vrg != null)
                {
                    try
                    {
                        string vid = vrg.v.VorgangId;
                        DataObject data = new(vid);
                        DragDrop.DoDragDrop(Planed, data, DragDropEffects.Move);
                    }
                    catch (NullReferenceException ex)
                    {

                        MessageBox.Show(ex.Message +"/n/n" + ex.InnerException.Message);
                    }
                    
                }
            }
        }

        private void Planed_Drop(object sender, DragEventArgs e)
        {
            string d = (string)e.Data.GetData(DataFormats.StringFormat);
            PlanMachine pl = DataContext as PlanMachine; pl?.ChangeProcessesCommand.Execute(d);
            (e.Source as DataGrid).Background = Brushes.White; 
            e.Handled = true;
        }

        private void Planed_DragEnter(object sender, DragEventArgs e)
        {
            (e.Source as DataGrid).Background = Brushes.Yellow;
            e.Handled = true;
        }

        private void Planed_DragLeave(object sender, DragEventArgs e)
        {
            (e.Source as DataGrid).Background = Brushes.White;
            e.Handled = true;
        }

        private void Planed_DragOver(object sender, DragEventArgs e)
        {
            
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
                e.Effects = DragDropEffects.Move;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            PlanMachine? pl = DataContext as PlanMachine;
            pl?.Exit();
        }

        private void Planed_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effects == DragDropEffects.Copy)
            {
                e.UseDefaultCursors = false;
                Mouse.SetCursor(Cursors.Hand);
            }
            else
                e.UseDefaultCursors = true;

            e.Handled = true;
        }

        private void HideDetails_Click(object sender, RoutedEventArgs e)
        {
            Planed.SelectedIndex = -1;
            e.Handled= true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CommandBindings.Add(new CommandBinding(
                ELCommands.OpenMachine, HandleOpenMachineExecuted, HandleOpenMachineCanExecute));
        }

        private void HandleOpenMachineCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void HandleOpenMachineExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Window m = new MachineView();
            m.DataContext = this.DataContext;
            m.Title = "Inventarnummer: " + (DataContext as PlanMachine).InventNo;
            //UIElement ele = this;
            //do
            //{
            //    ele = (UIElement)VisualTreeHelper.GetParent(ele);
            //} while (ele.GetValue(NameProperty) != "MPL" && ele != null);
            m.Owner = App.Current.MainWindow;
            m.Show();
        }
    }
}
