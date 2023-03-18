using System;
using System.ComponentModel;

namespace Lieferliste_WPF.Data.Models
{
    public abstract class Stripe : INotifyPropertyChanged
    {


        public abstract int Start { get; set; }
        public abstract int End { get; set; }
        public abstract string Comment { get; set; }
        public abstract bool ValidateStartEnd(int value);
        public abstract string StripeColor { get; }
        public abstract int Type { get; }



        public abstract int TimeLenght
        {
            get;
        }

        public TimeSpan StartTime
        {
            get { return TimeSpan.FromMinutes(Start); }
            set
            {
                Start = (int)value.TotalMinutes;
                NotifyPropertyChanged("TimeLenght");
                NotifyPropertyChanged("Start");
            }
        }
        public TimeSpan EndTime
        {
            get { return TimeSpan.FromMinutes(End); }
            set
            {
                End = (int)value.TotalMinutes;
                NotifyPropertyChanged("TimeLenght");
                NotifyPropertyChanged("End");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
                //PropertyChanged(this, new PropertyChangedEventArgs("DisplayMember"));
            }
        }
    }
}

