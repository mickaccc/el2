using El2Core.Models;
using El2Core.Utils;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModulePlanning.Specials
{
    public interface IShiftPlan
    {
    }
    public class ShiftPlan : IShiftPlan
    {
        private ImmutableArray<bool[]> weekPlan;
        public ImmutableArray<bool[]> WeekPlan => weekPlan;
        IContainerProvider container;
        private readonly int rid;

        public ShiftPlan(int rid, IContainerProvider container)
        {
            this.rid = rid;

            this.container = container;
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            var w = db.ShiftPlanDbs.SingleOrDefault(x => x.Ressources.Any(x => x.RessourceId == rid));
            if (w != null)
            {


                List<bool[]> days = [];
                var minutes = w.Su.Split(',');
                days.Add(GetBools(minutes));
                minutes = w.Mo.Split(',');
                days.Add(GetBools(minutes));
                minutes = w.Tu.Split(',');
                days.Add(GetBools(minutes));
                minutes = w.We.Split(',');
                days.Add(GetBools(minutes));
                minutes = w.Th.Split(',');
                days.Add(GetBools(minutes));
                minutes = w.Fr.Split(',');
                days.Add(GetBools(minutes));
                minutes = w.Sa.Split(',');
                days.Add(GetBools(minutes));

                weekPlan = days.ToImmutableArray();
            }
        }
        bool[] GetBools(string[] minutes)
        {
            bool[] bools = new bool[1440];
            for (int i = 0; i < minutes.Length; i += 2)
            {
                BoolsFill(ref bools, int.Parse(minutes[i]), int.Parse(minutes[i + 1]), true);
            }
            return bools;
        }

        void BoolsFill(ref bool[] bools, int startIndex, int endIndex, bool value)
        {
            for(int i = startIndex; i < endIndex; i++)
            {
                bools[i] = value;
            }
        }
 
        public DateTime GetEndDateTime(double processLength, DateTime start)
        {
            
            if(weekPlan.IsDefaultOrEmpty || processLength == 0) return start;
            bool[] tmpWeekPlan;
            var holi = container.Resolve<HolidayLogic>();
            int resultMinute = 0;
            double length = processLength;
            int startDay = (int)start.DayOfWeek;
            TimeSpan time = start.TimeOfDay;
            if (holi.IsHolyday(start)) { tmpWeekPlan = weekPlan[0]; } else tmpWeekPlan = weekPlan[startDay]; //set the start of the week
 
                for(int j = (int)time.TotalMinutes; j < tmpWeekPlan.Length; j++)
                {
                    if (tmpWeekPlan[j]) length--;
                    if (length <= 0) { resultMinute = j; break; }
                }

            while (holi.IsHolyday(start.AddDays(1))) { start = start.AddDays(1); } //move the start to => tomorrow is not holiday
            if (length > 0)
            {
                start = start.Date;
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
