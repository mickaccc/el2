using System.Windows;
using System.Windows.Controls;

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// Interaction logic for MachineActionView.xaml
    /// </summary>
    public partial class MachineActionView : UserControl
    {
        public MachineActionView()
        {
            InitializeComponent();
            //testMachine m = new testMachine();
        }

        private void Control_loaded(object sender, RoutedEventArgs e)
        {
            //ICollectionView cv = CollectionViewSource.GetDefaultView(Weeks.ItemsSource);
            //cv.GroupDescriptions.Add(new PropertyGroupDescription("CalendarWeek"));
        }

        private void Weeks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
