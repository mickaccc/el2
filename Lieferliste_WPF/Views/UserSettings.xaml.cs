using MaterialDesignThemes.Wpf;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class UserSettings : Grid
    {
        public UserSettings()
        {
            InitializeComponent();

        }

        private static void ModifyTheme(bool isDarkTheme)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);
        }

        private void ToggleButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var toggle = (ToggleButton)sender;
            ModifyTheme(toggle.IsChecked ?? false);
        }
    }
}
