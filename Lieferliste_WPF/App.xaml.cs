using ControlzEx.Theming;
using MahApps.Metro.Theming;
using System;
using System.Windows;
using System.Windows.Media;

namespace Lieferliste_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            //ThemeManager.Current.AddLibraryTheme(new LibraryTheme(
            //    new Uri("pack://application:,,,/MahAppsMetroThemesSample;component/CustomAccents/Light.Accent1.xaml"),
            //    MahAppsLibraryThemeProvider.DefaultInstance));
            //ThemeManager.Current.AddLibraryTheme(new LibraryTheme(
            //    new Uri("pack://application:,,,/MahAppsMetroThemesSample;component/CustomAccents/Dark.Accent1.xaml"),
            //    MahAppsLibraryThemeProvider.DefaultInstance));
            //ThemeManager.Current.AddLibraryTheme(new LibraryTheme(
            //    new Uri("pack://application:,,,/MahAppsMetroThemesSample;component/CustomAccents/Light.Accent2.xaml"),
            //    MahAppsLibraryThemeProvider.DefaultInstance));
            //ThemeManager.Current.AddLibraryTheme(new LibraryTheme(
            //    new Uri("pack://application:,,,/MahAppsMetroThemesSample;component/CustomAccents/Dark.Accent2.xaml"),
            //    MahAppsLibraryThemeProvider.DefaultInstance));
            base.OnStartup(e);

            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
            ThemeManager.Current.SyncTheme();
            var theme = ThemeManager.Current.AddLibraryTheme(
    new LibraryTheme(
        new Uri("pack://application:,,,/Lieferliste_WPF;component/Themes/Light.Accent2.xaml"),
        MahAppsLibraryThemeProvider.DefaultInstance
        )
    );
            // Create runtime themes
            ThemeManager.Current.AddTheme(new Theme("CustomDarkRed", "CustomDarkRed", "Dark", "Red", Colors.DarkRed, Brushes.DarkRed, true, false));
            ThemeManager.Current.AddTheme(new Theme("CustomLightRed", "CustomLightRed", "Light", "Red", Colors.DarkRed, Brushes.DarkRed, true, false));
            ThemeManager.Current.AddTheme(new Theme("CustomLightPurple", "CustomLightPurble", "Light", "Green", Colors.Purple, Brushes.MediumPurple, true, true));
            ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Dark", Colors.Red));
            ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Light", Colors.Red));

            ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Dark", Colors.GreenYellow));
            ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Light", Colors.GreenYellow));

            ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Dark", Colors.Indigo));
            ThemeManager.Current.ChangeTheme(this, ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Light", Colors.Indigo)));

            ThemeManager.Current.ChangeTheme(this, "Light.Olive");

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }
}
