
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace El2UserControls
{
    /// <summary>
    /// Interaction logic for MachineUserControl.xaml
    /// </summary>
    public partial class MachineUserControl : UserControl
    {
        
        public MachineUserControl()
        {           
            InitializeComponent();
            //Planed.SelectedIndex = -1;
        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //PlanMachine? pl = DataContext as PlanMachine;
            //pl?.Exit();
        }

        private void HideDetails_Click(object sender, RoutedEventArgs e)
        {
           // Planed.SelectedIndex = -1;
            e.Handled= true;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //CommandBindings.Add(new CommandBinding(
            //    ELCommands.OpenMachine, HandleOpenMachineExecuted, HandleOpenMachineCanExecute));
        }

        private void HandleOpenMachineCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void HandleOpenMachineExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Window m = new MachineView
            //{
            //    DataContext = this.DataContext,
            //    Title = "Inventarnummer: " + (DataContext as PlanMachine).InventNo,
            //    Owner = App.Current.MainWindow
            //};
            //m.Show();
        }
    }
}
