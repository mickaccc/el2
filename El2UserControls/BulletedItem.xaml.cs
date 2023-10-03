using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace El2UserControls
{
    /// <summary>
    /// Interaction logic for BulletedItem.xaml
    /// </summary>
    public partial class BulletedItem : UserControl
    {

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("BulletText", typeof(string), typeof(BulletedItem));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("BulletColor", typeof (Color), typeof(BulletedItem),
            new PropertyMetadata(Color.FromRgb(255,255,255)));
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
