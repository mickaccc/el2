using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for MachinePlan.xaml
    /// </summary>
    public partial class MachinePlan : UserControl
    {
        [SupportedOSPlatform("windows10.0")]
        public MachinePlan()
        {
            InitializeComponent();

        }

        private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ColumnDefinition? col = (ColumnDefinition?)FindName("ParkColumn");
            if (col != null) col.Width = new GridLength(120);
        }

        private void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ColumnDefinition? col = (ColumnDefinition?)FindName("ParkColumn");
            if (col != null) col.Width = new GridLength(20);
        }

    }
}
