using Lieferliste_WPF.Data.Models;
using Lieferliste_WPF.Interfaces;
using Lieferliste_WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace Lieferliste_WPF.View
{
    /// <summary>
    /// Interaction logic for RoleEdit.xaml
    /// </summary>
    public partial class RoleEdit : Page
    {
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
