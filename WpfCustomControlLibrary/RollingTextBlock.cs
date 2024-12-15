using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfCustomControlLibrary
{
    public class RollingTextBlock : ContentControl
    {
        static RollingTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(RollingTextBlock), new FrameworkPropertyMetadata(typeof(RollingTextBlock)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.Template != null)
            {
                _TextBlock = Template.FindName("PART_CalendarWeek", this) as TextBlock;
                _buttonUp = Template.FindName("PART_Up", this) as Button;
                _buttonDown = Template.FindName("PART_Down", this) as Button;
                _buttonUp.Command = UpDownCommand;
                _buttonDown.Command = UpDownCommand;
                if(_TextBlock != null && RollingRange != null)
                {
                    if (DefaultRollingPosition == RollingPosition.First) _TextBlock.Text = RollingRange.FirstOrDefault();
                    if (DefaultRollingPosition ==  RollingPosition.Last) _TextBlock.Text = RollingRange.LastOrDefault();
                }
            }
        }
        public override void BeginInit()
        {
            base.BeginInit();
            this.CommandBindings.Add(new CommandBinding(
                UpDownCommand, UpDownExecuted, UpDownCanExecute));
        }
        void UpDownExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if (_buttonUp.Equals(e.OriginalSource))
            {
                _TextBlock.Text = RollingRange.ElementAt(RollingRange.IndexOf(_TextBlock.Text) - 1);
            }
            if (_buttonDown.Equals(e.OriginalSource))
            {
                _TextBlock.Text = RollingRange.ElementAt(RollingRange.IndexOf(_TextBlock.Text) + 1);
            }
        }
        void UpDownCanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            if (_buttonUp.Equals(e.OriginalSource)) e.CanExecute = _TextBlock.Text != RollingRange.First();
            if (_buttonDown.Equals(e.OriginalSource)) e.CanExecute = _TextBlock.Text != RollingRange.Last();
        }

        private TextBlock? _TextBlock;
        private Button? _buttonUp;
        private Button? _buttonDown;
        private RoutedCommand? _UpDownCommand;
        protected RoutedCommand UpDownCommand => _UpDownCommand ??= new RoutedCommand();
        public List<string> RollingRange
        {
            get { return (List<string>)GetValue(RollingRangeProperty); }
            set { SetValue(RollingRangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RollingRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RollingRangeProperty =
            DependencyProperty.Register("RollingRange", typeof(List<string>), typeof(RollingTextBlock), new PropertyMetadata());



        public RollingPosition DefaultRollingPosition
        {
            get { return (RollingPosition)GetValue(DefaultRollingPositionProperty); }
            set { SetValue(DefaultRollingPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultRollingPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultRollingPositionProperty =
            DependencyProperty.Register("DefaultRollingPosition", typeof(RollingPosition), typeof(RollingTextBlock), new PropertyMetadata(RollingPosition.Last));
        public enum RollingPosition
        {
            First,
            Last
        }
    }
}
