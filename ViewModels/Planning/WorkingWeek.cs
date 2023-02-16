using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lieferliste_WPF.ViewModels;

namespace Lieferliste_WPF.Planning
{
    class WorkingWeek
    {
        private WorkingDayViewModel[] _workingDays = new WorkingDayViewModel[7];
        public WorkingDayViewModel Monday { get { return _workingDays[0]; } }
        public WorkingDayViewModel Tuesday { get { return _workingDays[1]; } }
        public WorkingDayViewModel Wednesday { get { return _workingDays[2]; } }
        public WorkingDayViewModel Thursday { get { return _workingDays[3]; } }
        public WorkingDayViewModel Friday { get { return _workingDays[4]; } }
        public WorkingDayViewModel Saturday { get { return _workingDays[5]; } }
        public WorkingDayViewModel Sunday { get { return _workingDays[6]; } }
        public WorkingDayViewModel[] getWorkingDays { get { return _workingDays; } }
        public Lieferliste_WPF.DateUtils.CalendarWeek CalendarWeek {get; private set;}

        public WorkingWeek(DateUtils.CalendarWeek CalendarWeek)
        {
            this.CalendarWeek = CalendarWeek;
            DateTime monday = DateUtils.GetMonday(CalendarWeek.Year, CalendarWeek.Week);
            for (int i = 0; i < _workingDays.Length; i++)
            {
                DateTime day = monday.AddDays(i);
                _workingDays[i] = new WorkingDayViewModel();
                _workingDays[i].Date = day;

            }
        }
    }
}
