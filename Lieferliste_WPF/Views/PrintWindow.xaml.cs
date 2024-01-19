using System.Windows;
using System.Windows.Documents;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        public PrintWindow(FixedDocument document)
        {

            InitializeComponent();
            PreviewD.Document = document;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
