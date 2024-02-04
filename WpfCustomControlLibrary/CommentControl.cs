using System;
using System.Globalization;
using System.Runtime.CompilerServices;
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
    public class CommentControl : Control
    {



        private void OnTextChanged(object sender, TextChangedEventArgs e) { ChangeText(); }

        private TextBox _textBox;
        private TextBlock _infoHeader;
        internal string _headerText;
        internal string _commentText;


        static CommentControl()
        {

            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommentControl), new FrameworkPropertyMetadata(typeof(CommentControl)));
        }


                   public string CommentString
        {
            get => (string)GetValue(CommentStringProperty);
            set => SetValue(CommentStringProperty, value);
        }
        public static readonly DependencyProperty CommentStringProperty = DependencyProperty.Register(nameof(CommentString),
                typeof(string), typeof(CommentControl),
                new PropertyMetadata(null, OnCommentStringChanged));


        public new double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(nameof(FontSize),
                typeof(double), typeof(CommentControl),
                new FrameworkPropertyMetadata(11.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));



        internal string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderText.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(CommentControl), new PropertyMetadata(""));



        internal string CommentText
        {
            get { return (string)GetValue(CommentTextProperty); }
            set { SetValue(CommentTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommentText.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CommentTextProperty =
            DependencyProperty.Register("CommentText", typeof(string), typeof(CommentControl), new PropertyMetadata(""));




        public string User
        {
            get { return (string)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for User.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(string), typeof(CommentControl), new PropertyMetadata(""));


        protected override Size MeasureOverride(Size constraint)
        {
            FormattedText txt = GetFormattedText();
            return new Size(txt.Width + 5, txt.Height + 5);
        }


        private FormattedText GetFormattedText()
        {
            return
                new FormattedText(_textBox.Text
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
            _infoHeader = Template.FindName("PART_CommentInfo", this) as TextBlock;
            _textBox.LostFocus += LostFocus;
        }
        private static void OnCommentStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string s)
            {
                if (string.IsNullOrEmpty(s) == false)
                {
                    var cmt = (CommentControl)d;
                    var st = s.Split((char)29);
                    if (st.Length == 2)
                    {
                        cmt.HeaderText = st[0];
                        cmt.CommentText = st[1];
                    }
                }
            }
        }
        private void LostFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt.Text.Length != 0)
            {
                CommentString = string.Format("[{0} - {1}]{2}{3}", User, DateTime.Now, (char)29, txt.Text);
            }
        }

        private void ChangeText()
        {

        }

    }
}
