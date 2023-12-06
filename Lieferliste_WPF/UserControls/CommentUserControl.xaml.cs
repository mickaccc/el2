using System.Windows;
using System.Windows.Controls;

namespace Lieferliste_WPF.UserControls
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


        public string Comment
        {
            get { return (string)GetValue(CommentProperty); }
            set { SetValue(CommentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Comment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommentProperty =
            DependencyProperty.Register("Comment", typeof(string), typeof(CommentUserControl),
                new PropertyMetadata(null));


        public bool Permission
        {
            get { return (bool)GetValue(PermissionProperty); }
            set { SetValue(PermissionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Permission.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PermissionProperty =
            DependencyProperty.Register("Permission", typeof(bool), typeof(CommentUserControl),
                new PropertyMetadata(false));

    }
}
