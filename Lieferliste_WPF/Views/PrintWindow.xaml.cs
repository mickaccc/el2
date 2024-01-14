using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
