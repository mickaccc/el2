using El2Core.Models;
using Lieferliste_WPF.ViewModels;
using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for MachinePlan.xaml
    /// </summary>
    public partial class MachinePlan : UserControl
    {
        [SupportedOSPlatform("windows7.0")]
        public MachinePlan()
        {
            InitializeComponent();

        }



        private void MPL_Unloaded(object sender, RoutedEventArgs e)
        {
            (Main.DataContext as MachinePlanViewModel).Exit();
        }

    }
}
