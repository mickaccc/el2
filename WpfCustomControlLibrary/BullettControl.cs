using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfCustomControlLibrary
{
    [TemplatePart(Name = "PART_Circle",
        Type = typeof(Ellipse))]
    public class BullettControl : Control
    {
        static BullettControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(BullettControl),
                new FrameworkPropertyMetadata(typeof(BullettControl)));
        }

        private Ellipse _ellipse;
        private Rectangle _rectangle;
        public Brush BullettColor
        {
            get { return (Brush)GetValue(BullettColorProperty); }
            set { SetValue(BullettColorProperty, value); }
        }

        public static readonly DependencyProperty BullettColorProperty =
            DependencyProperty.Register("BullettColor", typeof(Brush), typeof(BullettControl),
                new PropertyMetadata(Brushes.White));

        public string BullettText
        {
            get { return (string)GetValue(BullettTextProperty); }
            set { SetValue(BullettTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BullettText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BullettTextProperty =
            DependencyProperty.Register("BullettText", typeof(string), typeof(BullettControl), new PropertyMetadata(""));



        public int BullettShape
        {
            get { return (int)GetValue(BullettShapeProperty); }
            set { SetValue(BullettShapeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BullettShape.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BullettShapeProperty =
            DependencyProperty.Register("BullettShape", typeof(int), typeof(BullettControl),
                new PropertyMetadata(0, OnBulletShapeChanged));



        private static void OnBulletShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bul = (BullettControl)d;
            if (bul._rectangle != null)
            {
                if (e.NewValue is int n)
                {
                    if (n == 0) { bul._ellipse.Visibility = Visibility.Visible; bul._rectangle.Visibility = Visibility.Hidden; }
                    if (n == 1) { bul._ellipse.Visibility = Visibility.Hidden; bul._rectangle.Visibility = Visibility.Visible; }
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.Template != null)
            {
                _ellipse = Template.FindName("PART_Circle", this) as Ellipse;
                _rectangle = Template.FindName("PART_Square", this) as Rectangle;
            }
        }

    }
}
