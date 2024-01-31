using Lieferliste_WPF.ViewModels;
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
    }
}
