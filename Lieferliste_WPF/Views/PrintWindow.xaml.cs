using GongSolutions.Wpf.DragDrop;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps;

namespace Lieferliste_WPF.Views
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        Collection<string> installedPrinters;
        public PrintWindow(FixedDocument document)
        {

            InitializeComponent();
            PreviewD.Document = document;
            cmbPrinterSelection.SelectedItem = LocalPrintServer.GetDefaultPrintQueue().Name;
        }


        private void Executed_Print(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            PrintTicket ticket = new();
            LocalPrintServer printServer = new();
            PrintQueue queue = printServer.GetPrintQueue((string)cmbPrinterSelection.SelectedValue);

            ticket.Duplexing = Duplexing.OneSided;
            int cp;
            if (int.TryParse(copies.Text, out cp)) ticket.CopyCount = cp; else ticket.CopyCount = 1;
            ticket.PageOrientation = PageOrientation.Landscape;
            XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(queue);
            writer.Write((FixedDocument)PreviewD.Document, ticket);
        }
    }
}
