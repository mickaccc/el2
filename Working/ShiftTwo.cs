using Lieferliste_WPF.Entities;
using System;

namespace Lieferliste_WPF.Working
{
    public class ShiftTwo : Stripe
    {
        public const int MaxMinute = 1440;
        public const int MinMinute = 0;
        public int _type = 2;
        public String ToolTip { get { return "Spätschicht"; } }
        public override String Comment { get; set; }

        public ShiftTwo(TimeSpan start, TimeSpan end, String comment)
        {
            this.Start = (int)start.TotalMinutes;
            this.End = (int)end.TotalMinutes;
            this.Comment = comment;
        }
        public ShiftTwo(int start, int end, String comment)
        {
            this.Start = start;
            this.End = end;
            this.Comment = comment;
        }
        public override int Type { get { return _type; } }
        public override bool ValidateStartEnd(int value)
        {
            if (value < MinMinute || value > MaxMinute) throw new Exception("Out of DayLineMinutesBorder");

            return true;
        }
        public override String StripeColor
        {
            get
            {
                System.Drawing.Color c = Properties.Settings.Default.Stripe2;
                return "#" + c.Name.Substring(2).ToUpper();
            }
        }
        public override int Start { get; set; }
        public override int End { get; set; }
        public override int TimeLenght
        {
            get { return End - Start; }
        }
        public void buildShift(int Start, int End, int type, String comment)
        {
            this.Start = Start;
            this.End = End;

        }
    }
}
