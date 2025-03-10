using System;
using System.Windows;
using System.Windows.Controls;

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
    public class CommentControl : TextBox
    {


        private TextBox _textBox;
        private Canvas _canvas;


        static CommentControl()
        {

            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommentControl), new FrameworkPropertyMetadata(typeof(CommentControl)));
        }


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
            DependencyProperty.Register("HeaderText", typeof(string), typeof(CommentControl),
                new PropertyMetadata(default(string)));



        internal string CommentText
        {
            get { return (string)GetValue(CommentTextProperty); }
            set { SetValue(CommentTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommentText.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CommentTextProperty =
            DependencyProperty.Register("CommentText", typeof(string), typeof(CommentControl), new PropertyMetadata(default(string)));

        public string User
        {
            get { return (string)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for User.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(string), typeof(CommentControl), new PropertyMetadata(default(string)));



        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(CommentControl),
                new PropertyMetadata(false, OnEditableCallback));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = (Canvas)Template.FindName("PART_CanvasSize", this);
            _textBox = (TextBox)Template.FindName("PART_TextBox", this);
            _textBox.SizeChanged += txSizeChanged;
 
            LostFocus += OnLostFocus;  
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(Text) == false)
            {
                var st = Text.Split((char)29);
                if (st.Length == 2)
                {
                    HeaderText = st[0];
                    CommentText = st[1];
                }
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (_textBox.Text.Length != 0)
            {
                if (CommentText != _textBox.Text)
                {
                    var val = string.Format("[{0} - {1}]{2}{3}", User, DateTime.Now.ToShortDateString(), (char)29, _textBox.Text);
                    Text = val;
                }
            }
            else
            {
                SetValue(TextProperty, default);
            }
        }

        private static void OnEditableCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cmt = (CommentControl)d;
            cmt._textBox.IsEnabled = (bool)e.NewValue;
        }
 
        private void txSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var txt = sender as TextBox;
            _canvas.Width = e.NewSize.Width;
            _canvas.Height = 15 + e.NewSize.Height;
            
        }
    }
}
