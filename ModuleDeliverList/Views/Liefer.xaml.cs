using ModuleDeliverList.UserControls;
using System.Windows;
using System.Windows.Controls;

namespace ModuleDeliverList.Views
{
    /// <summary>
    /// Interaktionslogik für Lieferliste.xaml
    /// </summary>
    public partial class Liefer : UserControl
    {

        public Liefer()
        {
            InitializeComponent();
        }

        private void ucLiefer_GotFocus(object sender, RoutedEventArgs e)
        {
            var data = e.Source as LieferlisteControl;
            if (data != null)
                Lieferlist.SelectedItem = data.DataContext;
        }
    }
}
