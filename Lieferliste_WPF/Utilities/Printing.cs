﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Lieferliste_WPF.Planning;
using System.Globalization;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using Lieferliste_WPF.Views;
using System.Reflection.Metadata;
using System.Xml.Linq;
using System.Windows.Xps.Serialization;
using System.IO.Packaging;
using System.Printing;
using El2Core.Models;

namespace Lieferliste_WPF.Utilities
{
    static class Printing
    {
        public static void DoThePrint(FlowDocument document)
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
                PrintDialog dialog = new();
                dialog.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;

                docWriter.Write(paginator, dialog.PrintTicket);


            }

        }
        public static FlowDocument CreateFlowDocument(object parameter)
        {
            string Name;
            string? Second;
            string? Description;
            List<Vorgang> proces;
            if (parameter is PlanMachine plm)
            {
                Name = plm.Name ?? string.Empty;
                Second = plm.InventNo ?? string.Empty;
                proces = plm.Processes?.ToList() ?? [];
                Description = plm.Description ?? string.Empty;
            }
            else if (parameter is PlanWorker plw)
            {
                Name = plw.Name ?? string.Empty;
                Second = plw.PersNo.ToString() ?? string.Empty;
                proces = plw.Processes?.ToList() ?? [];
                Description = plw.Description ?? string.Empty;
            }
            else throw new InvalidDataException();

            FlowDocument fd = new FlowDocument();
            Paragraph p1 = new Paragraph(new Run(DateTime.Now.ToString("ddd, dd/MM/yyyy hh:mm")));
            p1.FontStyle = FontStyles.Normal;
            p1.FontFamily = new FontFamily("Microsoft Sans Serif");
            p1.FontSize = 12;
            fd.Blocks.Add(p1);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Name).Append(' ').AppendLine(Second).Append(Description);
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
            fd.ColumnWidth = 800;
            fd.IsColumnWidthFlexible = true;
            table.CellSpacing = 1;


            var headerList = proces.OrderBy(x => x.Spos).Select(x => new
            {
                x.Aid,
                ProcessingUom = string.Format("{0:d4}", x.Vnr),
                x.AidNavigation.Material,
                x.AidNavigation.MaterialNavigation?.Bezeichng,
                BeazeEinheit = x.AidNavigation.Quantity.ToString(),
                x.Text,
                RstzeEinheit = string.Format("{0} KW{1}", x.SpaetStart.GetValueOrDefault().ToString("dd/MM/yy"),
                        CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(x.SpaetStart.GetValueOrDefault(), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)),
                SpaetEnd = x.SpaetEnd.GetValueOrDefault().ToShortDateString(),
                x.BemT,
                WrtzeEinheit = string.Format("{0:F2}h", (((x.Beaze == null) ? 0 : x.Beaze) + ((x.Rstze == null) ? 0 : x.Rstze)) / 60)
            }).ToArray();

            int i = 0;
            foreach (var pr in headerList.First().GetType().GetProperties())
            {

                string head;
                switch (pr.Name)
                {
                    case "Aid": head = "Auftrags-\nnummer"; break;
                    case "ProcessingUom": head = "Vorgang"; break;
                    case "Material": head = "Material"; break;
                    case "Bezeichng": head = "Bezeichn- ung"; break;
                    case "BeazeEinheit": head = "Menge"; break;
                    case "Text": head = "Kurztext"; break;
                    case "RstzeEinheit": head = "SpätStart"; break;
                    case "SpaetEnd": head = "SpätEnd"; break;
                    case "BemT": head = "Bemerkung Teamleiter"; break;
                    case "WrtzeEinheit": head = "Dauer"; break;
                    default: head = "not Valid"; break;
                }
                r.Cells.Add(new TableCell(new Paragraph(new Run(head))));
                r.Cells[i].ColumnSpan = 1;
                r.Cells[i].Padding = new Thickness(2);

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

                    r.Cells.Add(new TableCell(new Paragraph(new Run(property.GetValue(row)?.ToString()))));
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

        public static void DoPrintPreview(FlowDocument doc)
        {
 
            PrintDialog dialog = new();
            dialog.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;

            doc.PagePadding = new Thickness(10.0, 20.0, 10.0, 10.0);
            var fix = Get_Fixed_From_FlowDoc(doc, dialog);
            var windows = new PrintWindow(fix);
            windows.ShowDialog();
        }
        private static string _previewWindowXaml =
    @"<Window
        xmlns ='http://schemas.microsoft.com/netfx/2007/xaml/presentation'
        xmlns:x ='http://schemas.microsoft.com/winfx/2006/xaml'
        Title ='Print Preview - @@TITLE'
        Height ='200' Width ='300'
        WindowStartupLocation ='CenterOwner'>
                      <DocumentViewer Name='dv1'/>
     </Window>";


        //public static void DoPreview(string title)
        //{
        //    string fileName = System.IO.Path.GetRandomFileName();


        //    FlowDocumentScrollViewer visual = (FlowDocumentScrollViewer)_parent.FindName("fdsv1");

        //    try
        //    {
        //        // write the XPS document
        //        using (XpsDocument doc = new XpsDocument(fileName, FileAccess.ReadWrite))
        //        {
        //            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
        //            writer.Write(visual);
        //        }

        //        // Read the XPS document into a dynamically generated
        //        // preview Window 
        //        using (XpsDocument doc = new XpsDocument(fileName, FileAccess.Read))
        //        {
        //            FixedDocumentSequence fds = doc.GetFixedDocumentSequence();

        //            string s = _previewWindowXaml;
        //            s = s.Replace("@@TITLE", title.Replace("'", "&apos;"));

        //            using (var reader = new System.Xml.XmlTextReader(new StringReader(s)))
        //            {
        //                Window preview = System.Windows.Markup.XamlReader.Load(reader) as Window;

        //                DocumentViewer dv1 = LogicalTreeHelper.FindLogicalNode(preview, "dv1") as DocumentViewer;
        //                dv1.Document = fds as IDocumentPaginatorSource;


        //                preview.ShowDialog();
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        if (File.Exists(fileName))
        //        {
        //            try
        //            {
        //                File.Delete(fileName);
        //            }
        //            catch
        //            {
        //            }
        //        }
        //    }
        //}
        public static FixedDocument Get_Fixed_From_FlowDoc(FlowDocument flowDoc, PrintDialog printDlg)
        {
            var fixedDocument = new FixedDocument();
            try
            {
                var pdlgPrint = printDlg ?? new PrintDialog();

                DocumentPaginator dpPages = (DocumentPaginator)((IDocumentPaginatorSource)flowDoc).DocumentPaginator;
                dpPages.ComputePageCount();
                PrintCapabilities capabilities = pdlgPrint.PrintQueue.GetPrintCapabilities(pdlgPrint.PrintTicket);

                for (int iPages = 0; iPages < dpPages.PageCount; iPages++)
                {
                    var page = dpPages.GetPage(iPages);
                    var pageContent = new PageContent();
                    var fixedPage = new FixedPage();

                    Canvas canvas = new Canvas();

                    VisualBrush vb = new VisualBrush(page.Visual);
                    vb.Stretch = Stretch.None;
                    vb.AlignmentX = AlignmentX.Left;
                    vb.AlignmentY = AlignmentY.Top;
                    vb.ViewboxUnits = BrushMappingMode.Absolute;
                    vb.TileMode = TileMode.None;
                    vb.Viewbox = new Rect(0, 0, capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                    FixedPage.SetLeft(canvas, 0);
                    FixedPage.SetTop(canvas, 0);

                    canvas.Width = capabilities.PageImageableArea.ExtentWidth;
                    canvas.Height = capabilities.PageImageableArea.ExtentHeight;
                    canvas.Background = vb;

                    fixedPage.Children.Add(canvas);

                    fixedPage.Height = pdlgPrint.PrintableAreaHeight;
                    fixedPage.HorizontalAlignment = HorizontalAlignment.Stretch;
                    pageContent.Child = fixedPage;
                    
                    fixedDocument.Pages.Add(pageContent);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return fixedDocument;
        }
        public static void SaveAsXps(string path, FlowDocument document)
            {
                using (Package package = Package.Open(path, FileMode.Create))
                {
                    using (var xpsDoc = new XpsDocument(
                        package, CompressionOption.Maximum))
                    {
                        var xpsSm = new XpsSerializationManager(
                            new XpsPackagingPolicy(xpsDoc), false);
                        DocumentPaginator dp =
                            ((IDocumentPaginatorSource)document).DocumentPaginator;
                        xpsSm.SaveAsXaml(dp);
                    }
                }
            }
        
    }
}
