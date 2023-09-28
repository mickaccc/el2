using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;

namespace El2Utilities.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class XML_FlowDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string xamlString = value as string;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
