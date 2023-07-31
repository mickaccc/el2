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


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            PlanMachine? pl = DataContext as PlanMachine;
            pl?.Exit();
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
            m.Owner = App.Current.MainWindow;
            m.Show();
        }
    }
}
