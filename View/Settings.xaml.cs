using Lieferliste_WPF.ViewModels;
using System.Windows;

namespace Lieferliste_WPF.View
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            this.DataContext = new SettingsViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
