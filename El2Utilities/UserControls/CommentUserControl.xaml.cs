using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace El2Core.UserControls
{
    /// <summary>
    /// Interaction logic for CommentUserControl.xaml
    /// </summary>
    public partial class CommentUserControl : UserControl
    {
        public CommentUserControl()
        {
            InitializeComponent();
        }


        public string? CommentString
        {
            get { return (string)GetValue(CommentStringProperty); }
            set { SetValue(CommentStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommentString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentStringProperty =
            DependencyProperty.Register("CommentString", typeof(string), typeof(CommentUserControl),
                new PropertyMetadata(default, OnCommentStringChanged));
         
        public string CommentText
        {
            get { return (string)GetValue(CommentTextProperty); }
            set { SetValue(CommentTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommentText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentTextProperty =
            DependencyProperty.Register("CommentText", typeof(string), typeof(CommentUserControl),
                new PropertyMetadata(default, OnCommentTextChanged));

        public string CommentInfo
        {
            get { return (string)GetValue(CommentInfoProperty); }
            set { SetValue(CommentInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommentInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentInfoProperty =
            DependencyProperty.Register("CommentInfo", typeof(string), typeof(CommentUserControl), new PropertyMetadata(default));



        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(CommentUserControl), new PropertyMetadata(default));


        public new double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        public static new readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(nameof(FontSize),
                typeof(double), typeof(CommentUserControl),
                new FrameworkPropertyMetadata(11.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));


        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(CommentUserControl), new PropertyMetadata(false));

        private static void OnCommentStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cmt = (CommentUserControl)d;
            if (e.NewValue is string s)
            {
                var sp = s.Split((char)29);
                if (sp.Length == 2)
                {
                    cmt.CommentInfo = sp[0];
                    cmt.CommentText = sp[1];
                }
            }
        }
        private static void OnCommentTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cmt = (CommentUserControl)d;
            if (((string)e.NewValue).Length > 0)
            {
                var info = $"[{cmt.UserName} - {DateTime.Now.ToShortDateString()}]";
                var text = (string)e.NewValue;
                cmt.CommentInfo = info;
                //cmt.CommentText = text;
                cmt.CommentString = string.Format("{0}{1}{2}", info, (char)29, text);
            }
            else
            {
                cmt.CommentInfo = string.Empty;
                cmt.CommentString = null;
            }
        }
    }
}
