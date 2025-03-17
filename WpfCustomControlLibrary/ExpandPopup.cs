using System.Windows;
using System.Windows.Controls;

namespace WpfCustomControlLibrary
{
    public class ExpandPopup : ItemsControl
    {
        static ExpandPopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpandPopup), new FrameworkPropertyMetadata(typeof(ExpandPopup)));
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ExpandPopup), new PropertyMetadata(false));


    }
}
