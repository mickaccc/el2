using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfCustomControlLibrary
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfCustomControlLibrary"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfCustomControlLibrary;assembly=WpfCustomControlLibrary"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class HintTextBox : TextBox
    {
        public static readonly DependencyProperty HintTextProperty;
        public static readonly DependencyProperty ShowTextProperty;
        public static new readonly DependencyProperty FontSizeProperty;
        public static new readonly DependencyProperty TextProperty;

        void OnTextChanged(object sender, TextChangedEventArgs e) { ChangeText();  }
        private TextBox _textBox;
        private TextBlock _placeHolder;
      

        static HintTextBox()
        {
            
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HintTextBox), new FrameworkPropertyMetadata(typeof(HintTextBox)));
            HintTextProperty = DependencyProperty.Register(nameof(HintText),
                typeof(string), typeof(HintTextBox),
                new FrameworkPropertyMetadata(null));

            ShowTextProperty = DependencyProperty.Register(nameof(ShowText),
                typeof(string), typeof(HintTextBox),
                new FrameworkPropertyMetadata(null));

            FontSizeProperty = DependencyProperty.Register(nameof(FontSize),
               typeof(double), typeof(HintTextBox),
               new FrameworkPropertyMetadata(11.0,
                   FrameworkPropertyMetadataOptions.AffectsMeasure));

            TextProperty = DependencyProperty.Register(nameof(Text),
                typeof(string), typeof(HintTextBox),
                new FrameworkPropertyMetadata("",
                   FrameworkPropertyMetadataOptions.AffectsMeasure));
        }
        public string Text
        {
            get => (string)GetValue(TextProperty); 
            set => SetValue(TextProperty, value); 
        }


        public new double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public string HintText
        {
            get => (string)GetValue(HintTextProperty);
            set => SetValue(HintTextProperty, value);
        }
        public string ShowText
        {
            get => (string)GetValue(ShowTextProperty);
            set => SetValue(ShowTextProperty, value);
        }
        protected override Size MeasureOverride(Size constraint)
        {
            if (Text != string.Empty)
            {
                FormattedText txt = GetFormattedText();
                return new Size(txt.Width + 5, txt.Height + 5);
            }
            return base.MeasureOverride(constraint);
        }

        [Obsolete]
        private FormattedText GetFormattedText()
        {
            return
                new FormattedText(this.Text
                , CultureInfo.InvariantCulture
                , FlowDirection.LeftToRight
                , new Typeface("Arial")
                , this.FontSize
                , Brushes.Black);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(Brushes.LightGray
                , null
                , new Rect(this.RenderSize));
            if (Text != null)
            {
                FormattedText txt = GetFormattedText();
                drawingContext.DrawText(txt, new Point(2.5, 2.5));
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBox = Template.FindName("PART_TextBox", this) as TextBox;
            _placeHolder = Template.FindName("PART_PlaceHolder", this) as TextBlock;

            if (_textBox != null) {
                ChangeText();
                _textBox.TextChanged += OnTextChanged;               
            }
            
        }
        private void ChangeText()
        {
            if(string.IsNullOrEmpty(_textBox.Text))
            {
                _placeHolder.Visibility = Visibility.Visible;
            }
            else
            {
                _placeHolder.Visibility = Visibility.Hidden;
            }

        }
    }
}
