using ControlzEx.Theming;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

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

        private void TestBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                RegExBox.Foreground = Brushes.Black;
                if (TestBox != null)
                {
                    if (string.IsNullOrEmpty(RegExBox.Text) == false)
                    {
                        var regex = new Regex(RegExBox.Text);
                        if (regex.IsMatch(TestBox.Text))
                        {
                            TestBox.Background = Brushes.LightGreen;
                        }
                        else
                        {
                            TestBox.Background = Brushes.White;
                        }
                    }
                    else TestBox.Background = Brushes.White;
                }
            }
            catch (RegexParseException) { RegExBox.Foreground = Brushes.Red; TestBox.Background = Brushes.White; }
            catch (System.Exception)
            {

                throw;
            }
        }

        private void tl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox send = sender as ListBox;
            send.SelectedItem = e.AddedItems[0];
        }
    }
}
