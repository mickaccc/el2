using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace El2UserControls
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
