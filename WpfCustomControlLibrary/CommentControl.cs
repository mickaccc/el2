using System;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

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
        private const bool DefaultValue = false;
        private TextBox _textBox;
        private Canvas _canvas;
        private bool _IsInUse = false;

        static CommentControl()
        {

            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommentControl), new FrameworkPropertyMetadata(typeof(CommentControl)));
        }



        public string CommentString
        {
            get { return (string)GetValue(CommentStringProperty); }
            set { SetValue(CommentStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommentString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentStringProperty =
            DependencyProperty.Register("CommentString", typeof(string), typeof(CommentControl),
                new PropertyMetadata(default, OnCommentChanged));

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
                new PropertyMetadata(DefaultValue, OnIsEditableChanged));

        private static void OnIsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cmt = (CommentControl)d;
            if (cmt._textBox != null)
            {
                cmt._textBox.IsReadOnly = !(bool)e.NewValue;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _canvas = (Canvas)Template.FindName("PART_CanvasSize", this);
            _textBox = (TextBox)Template.FindName("PART_TextBox", this);
            _textBox.IsEnabled = true;
            _textBox.IsReadOnly = true;
            _textBox.SizeChanged += txSizeChanged;
            GotFocus += OnGotFocus;
            LostFocus += OnLostFocus; 
            
        }


        private static void OnCommentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            var cmt = (CommentControl)d;
            if (!cmt._IsInUse)
            {
                if (e.NewValue is string s)
                {
                    var st = s.Split((char)29);
                    if (st.Length == 2)
                    {
                        cmt.HeaderText = st[0];
                        cmt.CommentText = st[1];
                    }
                }
            }
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            _IsInUse = true;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            _IsInUse = false;
            if (_textBox.Text.Length != 0)
            {
                if (CommentText != _textBox.Text)
                {
                    
                    HeaderText = $"[{User} - {DateTime.Now.ToShortDateString()}]";
                    CommentText = _textBox.Text ;
                    CommentString = string.Format("{0}{1}{2}", HeaderText, (char)29, _textBox.Text);
                }
            }
            else
            {
                HeaderText = string.Empty;
                CommentText = string.Empty;
                CommentString = default;
            }
            
        }
 
        private void txSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var txt = sender as TextBox;
            _canvas.Width = e.NewSize.Width;
            _canvas.Height = 15 + e.NewSize.Height;

        }
    }
}
