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

namespace ModuleUserControls
{
    /// <summary>
    /// Interaction logic for ShiftDayVisual.xaml
    /// </summary>
    public partial class ShiftDayVisual : UserControl
    {
        public ShiftDayVisual()
        {
            InitializeComponent();
        }

        #region Properties

        public string ShiftDayDef
        {
            get { return (string)GetValue(ShiftDayDefProperty); }
            set { SetValue(ShiftDayDefProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShiftDayDef.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShiftDayDefProperty =
            DependencyProperty.Register("ShiftDayDef", typeof(string), typeof(ShiftDayVisual), new PropertyMetadata(""));



        public string WeekDay
        {
            get { return (string)GetValue(WeekDayProperty); }
            set { SetValue(WeekDayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WeekDay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeekDayProperty =
            DependencyProperty.Register("WeekDay", typeof(string), typeof(ShiftDayVisual), new PropertyMetadata(""));

        #endregion

    }
}
