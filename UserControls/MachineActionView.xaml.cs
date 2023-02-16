using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lieferliste_WPF.Planning;
using System.ComponentModel;

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
