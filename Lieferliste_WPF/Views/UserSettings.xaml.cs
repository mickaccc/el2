using ControlzEx.Theming;
using System.Collections.Generic;
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
                TextBox testbox = (TextBox)FindName("TestBox");
                TextBox RegExBox = (TextBox)FindName("RegExBox");
                RegExBox.Foreground = Brushes.Black;
                if (testbox != null)
                {
                    if (string.IsNullOrEmpty(RegExBox.Text) == false)
                    {
                        var regex = new Regex(RegExBox.Text);
                        if (regex.IsMatch(testbox.Text))
                        {
                            testbox.Background = Brushes.LightGreen;
                        }
                        else
                        {
                            testbox.Background = Brushes.White;
                        }
                    }
                    else testbox.Background = Brushes.White;
                }
            }
            catch (RegexParseException)
            {
                TextBox testbox = (TextBox)FindName("TestBox");
                TextBox RegExBox = (TextBox)FindName("RegExBox");
                RegExBox.Foreground = Brushes.Red; testbox.Background = Brushes.White;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

    }
}
