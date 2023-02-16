
using System.Windows;
using System.Threading;
using System.Windows.Markup;
using System.Globalization;
namespace Lieferliste_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : Application
    {
        public App()
        {

        }

        protected override void OnStartup(StartupEventArgs e)
        {

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-AT"); ;

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-AT"); ;



            FrameworkElement.LanguageProperty.OverrideMetadata(

              typeof(FrameworkElement),

              new FrameworkPropertyMetadata(

                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));



            base.OnStartup(e);

        }


    }
}
