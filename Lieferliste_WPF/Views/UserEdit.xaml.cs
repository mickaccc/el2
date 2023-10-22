using Lieferliste_WPF.Interfaces;
using Lieferliste_WPF.ViewModels;
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
using System.Windows.Shapes;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for UserEdit.xaml
    /// </summary>
    public partial class UserEdit : Grid
    {
        public bool IsLoading { get; set; }
        public string Ident { get; set; }
        public UserEdit()
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

        private void U01_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}
