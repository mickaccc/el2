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
