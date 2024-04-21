using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using System.Xml.Xsl;

namespace El2Core.Utils
{
    public class TableBuilder
    {
        public void Build(AbstracatBuilder builder)
        {  builder.Build(); }
    }

    public abstract class AbstracatBuilder
    {
        protected List<string?[]>? context;
        public List<string?[]>? Context
        { get { return context; } }
        public abstract void Build();
        public abstract void SetContext(List<string?[]> context);
        public abstract string GetResult(string format);
        public abstract FlowDocument GetDoc();
    }
    public class FlowTableBuilder : AbstracatBuilder
    {
        string[] headers;
        FlowDocument doc;
        Table table;
        public FlowTableBuilder(string[] headers) { this.headers = headers; }
        public override void Build()
        {
            doc = new FlowDocument();
            doc.PageWidth = 700.0;
            doc.ColumnWidth = doc.PageWidth;
            table = new Table();
            TableRow row = new TableRow();
            TableRowGroup rowGroup = new TableRowGroup();

            foreach (var head in headers)
            {
                row.Cells.Add(new TableCell(new Paragraph(new Run(head)))
                {
                    BorderThickness = new Thickness(1, 0, 0, 1),
                    BorderBrush = Brushes.Black,
                    Background = Brushes.DarkGray,
                    Foreground = Brushes.White
                });
            }
            rowGroup.Rows.Add(row);
            table.RowGroups.Add(rowGroup);

            foreach(var body in context)
            {
                rowGroup = new TableRowGroup();
                row = new TableRow();
                foreach (var cell in body)
                {
                    if (row.Cells.Count < 5)
                    {
                        row.Cells.Add(new TableCell(new Paragraph(new Run(cell))));
                    }
                    else
                    {
                        var inl = new InlineUIContainer();
                        var txb = new TextBox
                        {
                            Text = cell,
                            IsReadOnly = false
                        };
                        inl.Child = txb;
                        var tc = new TableCell(new Paragraph(inl));
                        
                        row.Cells.Add(tc);
                    }
                }
                rowGroup.Rows.Add(row);
                table.RowGroups.Add(rowGroup);
            }
            doc.Blocks.Add(table);
        }
        private XslCompiledTransform LoadTransformResource(string path)
        {
            Uri uri = new Uri(path, UriKind.Relative);
            XmlReader xr = XmlReader.Create(Application.GetResourceStream(uri).Stream);
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xr);
            return xslt;
        }
        public override string GetResult(string format)
        {
            XslCompiledTransform ToHtmlTransform = LoadTransformResource("/FlowDocumentToXhtml.xslt");
            using (MemoryStream ms = new MemoryStream())
            {
                // write XAML out to a MemoryStream
                TextRange tr = new TextRange(
                    doc.ContentStart,
                    doc.ContentEnd);
                tr.Save(ms, DataFormats.Xaml);
                ms.Seek(0, SeekOrigin.Begin);

                // transform the contents of the MemoryStream to HTML
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    XmlWriterSettings xws = new XmlWriterSettings();
                    xws.OmitXmlDeclaration = true;
                    XmlReader xr = XmlReader.Create(ms);
                    XmlWriter xw = XmlWriter.Create(sw, xws);
                    ToHtmlTransform.Transform(xr, xw);
                }
                
                return sb.ToString();
            }
            
        }
        public override FlowDocument GetDoc()
        {
            return doc;
        }
        public override void SetContext(List<string?[]> context)
        {
            this.context = context;
        }
    }
    public class WordTableBuilder : AbstracatBuilder
    {
        public override void Build()
        {
    
            
        }

        public override FlowDocument GetDoc()
        {
            throw new NotImplementedException();
        }

        public override string GetResult(string format)
        {
            throw new NotImplementedException();
        }

        public override void SetContext(List<string?[]> context)
        {
            throw new NotImplementedException();
        }
    }
}
