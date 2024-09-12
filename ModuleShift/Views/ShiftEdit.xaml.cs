using El2Core.Services;
using ModuleShift.Services;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModuleShift.Views
{
    /// <summary>
    /// Interaction logic for ShiftEdit.xaml
    /// </summary>
    public partial class ShiftEdit : UserControl
    {
        public ShiftEdit()
        {
            InitializeComponent();
        }

        private void ShiftHeader_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsInitialized)
            {
                if (ShiftHeader.SelectedItem is WorkShiftService c)
                    ShiftDetails.ItemsSource = c.Items;
            }
        }

        private void ShiftDetails_Initialized(object sender, EventArgs e)
        {
            if (ShiftHeader.SelectedItem is WorkShiftService c)
                ShiftDetails.ItemsSource = c.Items;
        }
    }
}
