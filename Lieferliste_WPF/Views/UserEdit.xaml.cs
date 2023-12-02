using System.Windows.Controls;
using System.Windows.Input;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for UserEdit.xaml
    /// </summary>
    public partial class UserEdit : UserControl
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

    }
}
