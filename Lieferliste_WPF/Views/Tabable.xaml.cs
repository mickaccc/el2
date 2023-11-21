using System.Windows;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for Tabable.xaml
    /// </summary>
    public partial class Tabable : Window
    {
        public Tabable()
        {
            InitializeComponent();
        }

        //private void FrameworkElement_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if(e.ChangedButton == MouseButton.Left)
        //    {
        //        this.DragMove();
        //    }

        //}





        private void tabable_Loaded(object sender, RoutedEventArgs e)
        {
            this.MouseDown += delegate { DragMove(); };
            //this.PreviewMouseDown += delegate { DragMove(); };
        }
    }
}
