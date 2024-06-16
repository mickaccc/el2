using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace WpfCustomControlLibrary
{
    public class ShiftPlanVisual : Thumb
    {
        static ShiftPlanVisual()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ShiftPlanVisual),
                new FrameworkPropertyMetadata(typeof(ShiftPlanVisual)));
        }
        private Rectangle? Rectangle;
        private TextBlock?[] TextBlockes = new TextBlock[7];
        private Canvas?[] Canvases = new Canvas[7];


        public Dictionary<int, string> WeekMinutes
        {
            get { return (Dictionary<int, string>)GetValue(WeekMinutesProperty); }
            set { SetValue(WeekMinutesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WeekMinutes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeekMinutesProperty =
            DependencyProperty.Register("WeekMinutes", typeof(Dictionary<int, string>), typeof(ShiftPlanVisual),
                new PropertyMetadata(null, OnWeekMinutesChanged));



        public Dictionary<string, Block[]> VisualMinutes
        {
            get { return (Dictionary<string, Block[]>)GetValue(VisualMinutesProperty); }
            set { SetValue(VisualMinutesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisualMinutes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisualMinutesProperty =
            DependencyProperty.Register("VisualMinutes", typeof(Dictionary<string, Block[]>), typeof(ShiftPlanVisual), new PropertyMetadata(null));


        private static void OnWeekMinutesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ShiftPlan = (ShiftPlanVisual)d;
            if (ShiftPlan.VisualMinutes == null) { ShiftPlan.VisualMinutes = []; }
                for (int i = 0; i < ShiftPlan.WeekMinutes.Keys.Count; i++)
                {
                    var wkDay = DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)i);
                    String[] minute = ShiftPlan.WeekMinutes[i].Split(',');
                    bool[] bools = new bool[1440];
                    for (int j = 0; j < minute.Length; j += 2)
                    {
                        int start, end;
                        start = int.Parse(minute[j]);
                        end = int.Parse(minute[j + 1]);
                        bools.AsSpan(start, end-start).Fill(true);
                    }
                    var st = bools.AsSpan().IndexOfAnyInRange(false, true);
                    var en = bools.AsSpan().IndexOfAnyInRange(true, false);
                var di = bools.AsSpan().IndexOfAnyInRange(true, true);

                      var ss = Array.IndexOf(bools, true);

            }
            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.Template != null)
            {
                TextBlockes[0] = Template.FindName("PART_Sun", this) as TextBlock;
                TextBlockes[1] = Template.FindName("PART_Mon", this) as TextBlock;

                Canvases[0] = Template.FindName("PART_SunValue", this) as Canvas;
                Canvases[1] = Template.FindName("PART_MonValue", this) as Canvas;
            }
        }
        public struct Block
        {
            public int Start;
            public int Length;
            public bool Active;
            public Block(int start, int length, bool active) { Start = start; Length = length; Active = active; }
        }
    }
}
