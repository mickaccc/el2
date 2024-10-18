using System.Windows.Controls;
using System.Windows.Input;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for RoleEdit.xaml
    /// </summary>
    public partial class RoleEdit : UserControl
    {
        public bool IsLoading { get; set; }
        public string Ident { get; set; }
        public RoleEdit()
        {
            InitializeComponent();
        }

        private void HandleCloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "PKey" ||
                e.PropertyName == "Description" ||
                e.PropertyName == "Categorie" ||
                e.PropertyName == "PermissKey" ||
                e.PropertyName == "RoleName" ||
                e.PropertyName == "Permission")
            {
                e.Cancel = false;
            }
            else
            { e.Cancel = true; }
            if (e.PropertyName == "PermissKeyNavigation")
            {
                var pr = e.PropertyType.GetField("Description");

            }
        }
    }
}
