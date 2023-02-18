using Lieferliste_WPF.Entities;
using Lieferliste_WPF.Working;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lieferliste_WPF.ViewModels
{
    class WorkingDayViewModel : AbstractViewModel
    {
        private SortedSet<int> _workingMinutes;
        public Collection<Stripe> Kappa { get; set; }
        public List<ActionStripe> ActionStripes { get; set; }
        public DateTime Date { get; set; }
        public DateUtils.CalendarWeek CalendarWeek { get { return DateUtils.GetGermanCalendarWeek(Date); } }
        public DayOfWeek WeekDay { get { return Date.DayOfWeek; } }
        public WorkingDayViewModel()
        {
            Kappa = new Collection<Stripe>();
            ActionStripes = new List<ActionStripe>();
        }

        public DateUtils.CalendarWeek getCalendarWeek()
        {

            try
            {
                return DateUtils.GetGermanCalendarWeek(Date);
            }
            catch (ArgumentNullException e)
            {
                throw;
            }
        }

        public SortedSet<int> getWorkingMinutes()
        {
            if (_workingMinutes == null)
            {
                _workingMinutes = new SortedSet<int>();
                List<int> preg = new List<int>();

                try
                {
                    foreach (Stripe ws in Kappa)
                    {
                        if (ws != null)
                        {
                            switch (ws.Type)
                            {
                                case 0:
                                    for (int i = ws.Start; i <= ws.End; i++)
                                    {
                                        if (!preg.Contains(i))
                                        {
                                            preg.Add(i);
                                        }
                                    }
                                    break;

                                default:
                                    if (ws.Start < ws.End)
                                    {
                                        for (int i = ws.Start; i <= ws.End; i++)
                                        {
                                            if (!_workingMinutes.Contains(i))
                                            {
                                                _workingMinutes.Add(i);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    _workingMinutes.ExceptWith(preg);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
            return _workingMinutes;
        }
        public int getCapacity()
        {
            return getWorkingMinutes().Count();
        }
        public int getFreeCapacity()
        {
            var result =
              from f in ActionStripes
              select f.CalcLenght;

            return getCapacity() - result.Sum();
        }
    }
}
