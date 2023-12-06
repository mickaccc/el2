
using System.Windows;
using System.Windows.Controls;

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
            //Planed.SelectedIndex = -1;
        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //PlanMachine? pl = DataContext as PlanMachine;
            //pl?.Exit();
        }

        private void HideDetails_Click(object sender, RoutedEventArgs e)
        {
            Planed.SelectedIndex = -1;
            e.Handled = true;
        }

    }
}
