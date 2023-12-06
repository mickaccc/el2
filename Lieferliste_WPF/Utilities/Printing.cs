using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Lieferliste_WPF.Planning;

namespace Lieferliste_WPF.Utilities
{
    static class Printing
    {
        public static void DoThePrint(System.Windows.Documents.FlowDocument document)
        {
            // Clone the source document's content into a new FlowDocument.
            // This is because the pagination for the printer needs to be
            // done differently than the pagination for the displayed page.
            // We print the copy, rather that the original FlowDocument.
            System.IO.MemoryStream s = new System.IO.MemoryStream();
            TextRange source = new TextRange(document.ContentStart, document.ContentEnd);
            source.Save(s, DataFormats.Xaml);
            FlowDocument copy = new FlowDocument();
            TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
            dest.Load(s, DataFormats.Xaml);

            // Create a XpsDocumentWriter object, implicitly opening a Windows common print dialog,
            // and allowing the user to select a printer.

            // get information about the dimensions of the seleted printer+media.
            System.Printing.PrintDocumentImageableArea ia = null;
            System.Windows.Xps.XpsDocumentWriter docWriter = System.Printing.PrintQueue.CreateXpsDocumentWriter(ref ia);

            if (docWriter != null && ia != null)
            {
                DocumentPaginator paginator = ((IDocumentPaginatorSource)copy).DocumentPaginator;

                // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
                paginator.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
                Thickness t = new Thickness(72);  // copy.PagePadding;
                copy.PagePadding = new Thickness(
                                 Math.Max(ia.OriginWidth, t.Left),
                                   Math.Max(ia.OriginHeight, t.Top),
                                   Math.Max(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth), t.Right),
                                   Math.Max(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight), t.Bottom));

                copy.ColumnWidth = double.PositiveInfinity;
                //copy.PageWidth = 528; // allow the page to be the natural with of the output device

                // Send content to the printer.
                PrintDialog dialog = new PrintDialog();
                dialog.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
                docWriter.Write(paginator, dialog.PrintTicket);
            }

        }
        public static FlowDocument CreateFlowDocument(object parameter)
        {
            PlanMachine plm = (PlanMachine)parameter;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(DateTime.Now.ToLongDateString()).Append(" --- ").Append(DateTime.Now.ToLongTimeString());
            FlowDocument fd = new FlowDocument();
            Paragraph p1 = new Paragraph(new Run(stringBuilder.ToString()));
            p1.FontStyle = FontStyles.Normal;
            p1.FontFamily = new FontFamily("Microsoft Sans Serif");
            p1.FontSize = 12;
            fd.Blocks.Add(p1);
            stringBuilder.Clear();
            stringBuilder.Append(plm.Name).Append(' ').Append(plm.InventNo);
            Paragraph p = new Paragraph(new Run(stringBuilder.ToString()));
            p.FontStyle = FontStyles.Normal;
            p.FontWeight = FontWeights.Bold;
            p.FontFamily = new FontFamily("Segoe UI");
            p.FontSize = 18;
            fd.Blocks.Add(p);

            Table table = new Table();
            TableRowGroup tableRowGroup = new TableRowGroup();
            TableRow r = new TableRow();

            fd.BringIntoView();

            fd.TextAlignment = TextAlignment.Center;
            fd.ColumnWidth = 500;
            table.CellSpacing = 0;


            var headerList = plm.Processes.OrderBy(x => x.Spos).Select(x => new
            {
                x.Aid,
                x.AidNavigation.Material,
                x.AidNavigation.MaterialNavigation?.Bezeichng,
                x.AidNavigation.Quantity,
                x.Text,
                x.BemT
            }).ToArray();

            int i = 0;
            foreach (var pr in headerList.First().GetType().GetProperties())
            {

                string head;
                switch (pr.Name)
                {
                    case "Aid": head = "Auftragsnummer"; break;
                    case "Material": head = "Material"; break;
                    case "Bezeichng": head = "Bezeichnung"; break;
                    case "Quantity": head = "Menge"; break;
                    case "Text": head = "Kurztext"; break;
                    case "BemT": head = "Kommentar"; break;
                    default: head = "not Valid"; break;
                }
                r.Cells.Add(new TableCell(new Paragraph(new Run(head))));
                r.Cells[i].ColumnSpan = 1;
                r.Cells[i].Padding = new Thickness(4);

                r.Cells[i].BorderBrush = Brushes.Black;
                r.Cells[i].FontWeight = FontWeights.Bold;
                r.Cells[i].Background = Brushes.DarkGray;
                r.Cells[i].Foreground = Brushes.White;
                r.Cells[i].BorderThickness = new Thickness(1, 1, 1, 1);
                ++i;
            }
            tableRowGroup.Rows.Add(r);
            table.RowGroups.Add(tableRowGroup);
            foreach (var row in headerList)
            {

                table.BorderBrush = Brushes.Gray;
                table.BorderThickness = new Thickness(1, 1, 0, 0);
                table.FontStyle = FontStyles.Normal;
                table.FontFamily = new FontFamily("Arial");
                table.FontSize = 13;
                tableRowGroup = new TableRowGroup();
                r = new TableRow();
                i = 0;
                foreach (var property in row.GetType().GetProperties())
                {

                    r.Cells.Add(new TableCell(new Paragraph(new Run((string)property.GetValue(row)))));
                    r.Cells[i].ColumnSpan = 1;
                    r.Cells[i].Padding = new Thickness(1);
                    r.Cells[i].BorderBrush = Brushes.DarkGray;
                    r.Cells[i].BorderThickness = new Thickness(0, 0, 1, 1);
                    ++i;
                }

                tableRowGroup.Rows.Add(r);
                table.RowGroups.Add(tableRowGroup);
            }
            fd.Blocks.Add(table);

            return fd;


        }
    }
}
