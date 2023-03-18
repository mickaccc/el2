using Lieferliste_WPF.Data.Models;
using System;

namespace Lieferliste_WPF.Working
{
    public class StripePause : Stripe
    {
        public const int MaxMinute = 1440;
        public const int MinMinute = 0;
        private int _type = 0;
        public String ToolTip { get; set; }
        public StripePause(TimeSpan start, TimeSpan end, String comment)
        {
            this.Start = (int)start.TotalMinutes;
            this.End = (int)end.TotalMinutes;
        }
        public StripePause(int start, int end)
        {
            this.Start = start;
            this.End = end;
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
                System.Drawing.Color c = Properties.Settings.Default.Pause;
                return "#" + c.Name.Substring(2).ToUpper();
            }
        }

        public override int Start { get; set; }
        public override int End { get; set; }
        public override string Comment { get; set; }
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
