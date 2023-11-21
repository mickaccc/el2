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
    public class ClearableHintTextBox : TextBox
    {
        public static readonly DependencyProperty ClHintTextProperty;
        public static readonly DependencyProperty ClShowTextProperty;
        public static readonly DependencyProperty FontSizeProperty;
        public static readonly DependencyProperty TextProperty;

        private void OnTextChanged(object sender, TextChangedEventArgs e) { ChangeText(); }
        private TextBox _textBox;
        private TextBlock _placeHolder;
        private Button _button;

        static ClearableHintTextBox()
        {

            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClearableHintTextBox), new FrameworkPropertyMetadata(typeof(ClearableHintTextBox)));
            ClHintTextProperty = DependencyProperty.Register(nameof(ClHintText),
                typeof(string), typeof(ClearableHintTextBox),
                new FrameworkPropertyMetadata(null));

            ClShowTextProperty = DependencyProperty.Register(nameof(ClShowText),
                typeof(string), typeof(ClearableHintTextBox),
                new FrameworkPropertyMetadata(null));


            FontSizeProperty = DependencyProperty.Register(nameof(FontSize),
                typeof(double), typeof(ClearableHintTextBox),
                new FrameworkPropertyMetadata(11.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        }


        public new double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public string ClHintText
        {
            get => (string)GetValue(ClHintTextProperty);
            set => SetValue(ClHintTextProperty, value);
        }
        public string ClShowText
        {
            get => (string)GetValue(ClShowTextProperty);
            set => SetValue(ClShowTextProperty, value);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            FormattedText txt = GetFormattedText();
            return new Size(txt.Width + 5, txt.Height + 5);
        }


        private FormattedText GetFormattedText()
        {
            return
                new FormattedText(this.Text
                , CultureInfo.InvariantCulture
                , FlowDirection.LeftToRight
                , new Typeface("Arial")
                , this.FontSize
                , Brushes.Black
                , VisualTreeHelper.GetDpi(this).PixelsPerDip);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(Brushes.LightGray
                , null
                , new Rect(this.RenderSize));
            FormattedText txt = GetFormattedText();
            drawingContext.DrawText(txt, new Point(2.5, 2.5));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _textBox = Template.FindName("PART_TextBox", this) as TextBox;
            _placeHolder = Template.FindName("PART_PlaceHolder", this) as TextBlock;
            _button = Template.FindName("PART_Clear", this) as Button;

            if (_button != null)
            {
                _button.Click += OnClick;
            }

            if (_textBox != null)
            {
                ChangeText();
                _textBox.TextChanged += OnTextChanged;
            }

        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            _textBox.Clear();
        }

        private void ChangeText()
        {
            if (string.IsNullOrEmpty(_textBox.Text))
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
