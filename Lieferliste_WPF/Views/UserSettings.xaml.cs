using ControlzEx.Theming;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class UserSettings : UserControl
    {
        public UserSettings()
        {
            InitializeComponent();

        }


        private void AccentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTheme = e.AddedItems.OfType<Theme>().FirstOrDefault();
            if (selectedTheme != null)
            {
                ThemeManager.Current.ChangeTheme(Application.Current, selectedTheme);
                Application.Current?.MainWindow?.Activate();
            }
        }
    }
}
