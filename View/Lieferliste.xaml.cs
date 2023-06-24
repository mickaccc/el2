using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lieferliste_WPF.UserControls;
using Lieferliste_WPF.ViewModels;

namespace Lieferliste_WPF.View
{
    /// <summary>
    /// Interaktionslogik für Lieferliste.xaml
    /// </summary>
    public partial class Lieferliste : Page
    {
        
        public Lieferliste()
        {
            InitializeComponent();
            //Lieferlist.ItemsSource = new LieferViewModel().Orders;


        }


        private void Border_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void showOrder(object sender, MouseButtonEventArgs e)
        {


            if (true)
            {
                Window wnd = new Tabable();

                wnd.Owner = Application.Current.Windows[0];
                LieferlisteControl? lc = e.Source as LieferlisteControl;
                OrderView oview = new OrderView(lc.Aid);
                oview.Focusable = true;
                wnd.Content = oview;
                wnd.Tag = "tabable";
                wnd.Show();  
            }


        }

        private void lieferListe_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
