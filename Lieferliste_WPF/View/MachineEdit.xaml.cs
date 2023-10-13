using System.Windows.Controls;
using System.Windows.Input;

namespace Lieferliste_WPF.View
{
    /// <summary>
    /// Interaction logic for MachineEdit.xaml
    /// </summary>
    public partial class MachineEdit : Grid
    {
        public bool IsLoading { get; set; }
        public string Ident { get; set; }
        public MachineEdit()
        {
            InitializeComponent();

        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{

        //    CommandBindings.Add(new CommandBinding(
        //        ApplicationCommands.Close, HandleCloseExecuted,
        //        HandleCloseCanExecute));

        //}

        private void HandleCloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if(e.PropertyName == "RessourceId"
                || e.PropertyName == "Sort")
            {
                e.Cancel = true;
            }
            else if(e.PropertyName == "RessName")
            {
                e.Column.Header = "Name";
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
