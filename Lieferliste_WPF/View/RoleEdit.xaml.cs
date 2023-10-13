using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lieferliste_WPF.View
{
    /// <summary>
    /// Interaction logic for RoleEdit.xaml
    /// </summary>
    public partial class RoleEdit : Grid
    {
        public bool IsLoading { get; set; }
        public string Ident { get; set; }
        public RoleEdit()
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



        private void ListUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }


        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "PKey" ||
                e.PropertyName == "Description" ||
                e.PropertyName == "Categorie" ||
                e.PropertyName == "PermissionKey" ||
                e.PropertyName == "RoleKey" ||
                e.PropertyName == "PermissionKeyNavigation")
            {
                e.Cancel = false;
            }
            else
            { e.Cancel = true; }
            if (e.PropertyName == "PermissionKeyNavigation")
            {
                    var pr = e.PropertyType.GetField("Description");
                    Debug.WriteLine(pr);
            }
        }
    }
}
