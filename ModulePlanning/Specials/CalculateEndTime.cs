using El2Core.Models;
using El2Core.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Immutable;
using System.Globalization;

namespace ModulePlanning.Specials
{
    public interface IShiftPlan
    {
    }
    public class ShiftPlanService : IShiftPlan
    {
        private ImmutableArray<bool[]> weekPlan;
        private Dictionary<int, ImmutableArray<bool[]>> weekPlans = [];
        IContainerProvider container;
        private readonly int rid;
        private HolidayLogic holidayLogic;
        private List<Stopage> stoppages;
        private bool repeat;

        public ShiftPlanService(int rid, IContainerProvider container)
        {
            this.rid = rid;

            this.container = container;
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            stoppages = db.Stopages.AsNoTracking().Where(x => x.Rid == rid).ToList();
            var scal = db.ShiftCalendars
                .Include(x => x.ShiftCalendarShiftPlans)
                .ThenInclude(x => x.Plan)
                .SingleOrDefault(x => x.Ressources.Any(x => x.RessourceId == rid));
            if (scal != null)
            {
                repeat = scal.Repeat;
                foreach (var s in scal.ShiftCalendarShiftPlans.OrderBy(x => x.YearKw))
                {
                    List<bool[]> days = new List<bool[]>();
                    Byte[] bytes;
                    bool[] sbools = new bool[1440];
                    bytes = s.Plan.Sun;
                    BitArray bitArray = new BitArray(bytes);
                    bitArray.CopyTo(sbools, 0);
                    days.Add(sbools);

                    bytes = s.Plan.Mon;
                    bitArray = new BitArray(bytes);
                    bool[] mbools = new bool[1440];
                    bitArray.CopyTo(mbools, 0);
                    days.Add(mbools);

                    bytes = s.Plan.Tue;
                    bitArray = new BitArray(bytes);
                    bool[] tubools = new bool[1440];
                    bitArray.CopyTo(tubools, 0);
                    days.Add(tubools);

                    bytes = s.Plan.Wed;
                    bitArray = new BitArray(bytes);
                    bool[] wbools = new bool[1440];
                    bitArray.CopyTo(wbools, 0);
                    days.Add(wbools);

                    bytes = s.Plan.Thu;
                    bitArray = new BitArray(bytes);
                    bool[] thbools = new bool[1440];
                    bitArray.CopyTo(thbools, 0);
                    days.Add(thbools);

                    bytes = s.Plan.Fre;
                    bitArray = new BitArray(bytes);
                    bool[] fbools = new bool[1440];
                    bitArray.CopyTo(fbools, 0);
                    days.Add(fbools);

                    bytes = s.Plan.Sat;
                    bitArray = new BitArray(bytes);
                    bool[] sabools = new bool[1440];
                    bitArray.CopyTo(sabools, 0);
                    days.Add(sabools);

                    int key = int.Parse(s.YearKw);
                    weekPlans.Add(key, [.. days]);
                }
            }          
            
            holidayLogic = container.Resolve<HolidayLogic>();
        }
        //public bool InclusiveBetween(this IComparable a, IComparable b, IComparable c)
        //{
        //    return a.CompareTo(b) >= 0 && a.CompareTo(c) <= 0;
        //}

        public DateTime GetEndDateTime(double processLength, DateTime start)
        {
            int key = int.Parse(string.Concat(start.Year.ToString(),
                    CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(start, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)));
            if (repeat)
            {
                var count = weekPlans.Count;
                var diff = key - weekPlans.Keys.First();
                var mod = diff % count;
                var k = weekPlans.Keys.ElementAt(mod);
                weekPlan = weekPlans[k];
            }
            else if (weekPlans.TryGetValue(key, out weekPlan) == false) return start;
            
            var stop = stoppages.FirstOrDefault(x => x.Starttime < start && start < x.Endtime);
            if (stop != null) { processLength += stop.Starttime.Subtract(start).TotalMinutes; start = stop.Endtime; }

            if (weekPlan.IsDefaultOrEmpty || processLength == 0) return start;
            bool[] tmpWeekPlan;
            int resultMinute = 0;
            double length = processLength;
            int startDay = (int)start.DayOfWeek;
            TimeSpan time = start.TimeOfDay;
            if (holidayLogic.IsHolyday(start)) { tmpWeekPlan = weekPlan[0]; } else tmpWeekPlan = weekPlan[startDay]; //set the start of the week

            for (int j = (int)time.TotalMinutes; j < tmpWeekPlan.Length; j++)
            {
                if (tmpWeekPlan[j]) length--;
                if (length <= 0) { resultMinute = j; break; }
            }
            start = start.Date;
            while (holidayLogic.IsHolyday(start.AddDays(1))) { start = start.AddDays(1); } //move the start to => tomorrow is not holiday
            if (length > 0)
            {
                start = GetEndDateTime(length, start.AddDays(1));
            }
            else
            {
                start = start.AddMinutes(resultMinute);
            }
            
            return start;
        }
    }
}
