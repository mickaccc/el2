using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Annotations.Storage;
using System.Windows.Annotations;
using Microsoft.Win32.SafeHandles;
using System.Net;

namespace Lieferliste_WPF.View
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class RichTextEditor : Window
    {
        private AnnotationService _annotService;
        private FlowDocumentReader docViewer =new();
        private FileStream _annotStream;
        private XmlStreamStore _annotStore;
        

        public RichTextEditor()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            
        }
        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            AnnotationService service = new AnnotationService(docViewer);
            _annotStream = new FileStream("annot.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            XmlStreamStore store = new XmlStreamStore(_annotStream);
            store.AutoFlush = true;
            service.Enable(store);
            
            LoadXamlPackage(this.DataContext as MemoryStream);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            SaveXamlPackage(this.DataContext as MemoryStream);
            AnnotationService service =
                AnnotationService.GetService(docViewer);
            service.Disable();
            _annotStream.Close();
        }
        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = richTB.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = richTB.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = richTB.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = richTB.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;
            temp = richTB.Selection.GetPropertyValue(Inline.FontSizeProperty);
            cmbFontSize.Text = temp.ToString();
        }
        // Save XAML in RichTextBox to a file specified by _fileName
        void SaveXamlPackage(MemoryStream? memoryStream)
        {
            
            if (memoryStream != null)
            {
                TextRange range;
                range = new TextRange(richTB.Document.ContentStart, richTB.Document.ContentEnd);
                range.Save(memoryStream, DataFormats.Rtf);
                string str = range.Text;
                
            }
 
        }

        // Load XAML into RichTextBox from a file specified by _fileName
        void LoadXamlPackage(MemoryStream? memoryStream)
        {
            if (memoryStream != null)
            {
                if (memoryStream.Length > 0)
                {
                    TextRange range;
                    range = new TextRange(richTB.Document.ContentStart, richTB.Document.ContentEnd);
                    range.Load(memoryStream, DataFormats.Rtf);
                    Paragraph p = new Paragraph();
                    Run run = new()
                    {
                        Text = "Hallo äh"
                    };
                    p.Inlines.Add(run);
                    richTB.Document.Blocks.Clear();
                    richTB.Document.Blocks.Add(p);
                }
            }
        }
        //private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    OpenFileDialog dlg = new OpenFileDialog();
        //    dlg.Filter = "Rich Text Format (*.xaml)|*.xaml|All files (*.*)|*.*";
        //    if (dlg.ShowDialog() == true)
        //    {
        //        FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
        //        TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
        //        range.Load(fileStream, DataFormats.XamlPackage);
        //    }
        //}

        //private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    SaveFileDialog dlg = new SaveFileDialog();
        //    dlg.Filter = "Rich Text Format (*.xaml)|*.xaml|All files (*.*)|*.*";
        //    if (dlg.ShowDialog() == true)
        //    {
        //        FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
        //        TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
        //        range.Save(fileStream, DataFormats.XamlPackage);
        //    }
        //}

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                richTB.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            richTB.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
        }
        private void StartAnnotations()
        {
            // If there is no AnnotationService yet, create one.
            if (_annotService == null)
                // docViewer is a document viewing control named in Window1.xaml.
                _annotService = new AnnotationService(docViewer);

            // If the AnnotationService is currently enabled, disable it.
            if (_annotService.IsEnabled == true)
                _annotService.Disable();

            // Open a stream to the file for storing annotations.
            _annotStream = new FileStream(
                "annotation.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);

            // Create an AnnotationStore using the file stream.
            _annotStore = new XmlStreamStore(_annotStream);

            // Enable the AnnotationService using the new store.
            _annotService.Enable(_annotStore);
        }
    }
}
