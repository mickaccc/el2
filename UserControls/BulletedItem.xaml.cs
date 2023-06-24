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

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// Interaction logic for BulletedItem.xaml
    /// </summary>
    public partial class BulletedItem : UserControl
    {

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("BulletText", typeof(string), typeof(BulletedItem));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("BulletColor", typeof (Color), typeof(BulletedItem));
        public string BulletText
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public Color BulletColor
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public BulletedItem()
        {
            InitializeComponent();
            
        }
    }
}
