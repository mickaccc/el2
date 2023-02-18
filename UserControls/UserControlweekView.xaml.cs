using System;
using System.Windows.Controls;

namespace Lieferliste_WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UserControlWeekView.xaml
    /// </summary>
    public partial class WeekView : UserControl
    {
        private Lieferliste_WPF.DateUtils.CalendarWeek _kw;
        public Lieferliste_WPF.DateUtils.CalendarWeek KW { get { return _kw; } }


        public WeekView(int year, int week)
        {

            InitializeComponent();
            setKW(year, week);
        }
        public void setKW(int year, int week)
        {
            this.lblKW.Content = week.ToString() + " / " + year.ToString().Substring(2);
            DateTime dt = DateUtils.GetMonday(year, week);
            _kw = new DateUtils.CalendarWeek(year, week);

            this.dayView1.DateDT = dt.Date;
            this.dayView2.DateDT = dt.AddDays(1).Date;
            this.dayView3.DateDT = dt.AddDays(2).Date;
            this.dayView4.DateDT = dt.AddDays(3).Date;
            this.dayView5.DateDT = dt.AddDays(4).Date;
            this.dayView6.DateDT = dt.AddDays(5).Date;
            this.dayView7.DateDT = dt.AddDays(6).Date;

        }

    }

}
