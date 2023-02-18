using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Lieferliste_WPF.myConverters
{
    [ValueConversion(typeof(String), typeof(String))]
    public class XML_FlowDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String xamlString = value as String;
            if (xamlString == null) { return value; }
            if (xamlString.StartsWith("<") && xamlString.EndsWith(">"))
            {

                return xamlString;
            }
            else
            {
                Paragraph myParagraph = new Paragraph();
                myParagraph.Inlines.Add(new Run(xamlString));
                FlowDocument myFlowDocument = new FlowDocument();
                myFlowDocument.Blocks.Add(myParagraph);

                var r = XamlWriter.Save(myFlowDocument);
                return r;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
