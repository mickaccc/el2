using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

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
        public abstract Table GetResult();
    }
    public class FlowTableBuilder : AbstracatBuilder
    {
        string[] headers;
        object[] fields;
        FlowDocument doc;
        Table table;
        public FlowTableBuilder(string[] headers, object[] fields) { this.headers = headers; this.fields = fields; }
        public override void Build()
        {
            doc = new FlowDocument();
            doc.PageWidth = 400.0;
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
                foreach (var cell in body.GetType().GetProperties())
                {
                    row.Cells.Add(new TableCell(new Paragraph(new Run(cell.GetValue(body)?.ToString()))));
                }
                rowGroup.Rows.Add(row);
                table.RowGroups.Add(rowGroup);
            }
        }

        public override Table GetResult()
        {
            return table;
        }

        public override void SetContext(List<string?[]> context)
        {
            this.context = context;
        }
    }
}
