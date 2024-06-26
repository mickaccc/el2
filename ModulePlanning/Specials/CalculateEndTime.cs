using El2Core.Models;
using El2Core.Utils;
using Prism.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;

namespace ModulePlanning.Specials
{
    public interface IShiftPlan
    {
    }
    public class ShiftPlanService : IShiftPlan
    {
        private ImmutableArray<bool[]> weekPlan;
        public ImmutableArray<bool[]> WeekPlan => weekPlan;
        IContainerProvider container;
        private readonly int rid;
        private HolidayLogic holidayLogic;

        public ShiftPlanService(int rid, IContainerProvider container)
        {
            this.rid = rid;

            this.container = container;
            using var db = container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();

            //var w = db.ShiftPlans.SingleOrDefault(x => x.Ressources.Any(x => x.RessourceId == rid));
            var s = db.ShiftPlans.SingleOrDefault(x => x.Id == 1);
            List<bool[]> days = new List<bool[]>();
            Byte[] bytes;
            bool[] sbools = new bool[1440];
            bytes = s.Sun;
            BitArray bitArray = new BitArray(bytes);
            bitArray.CopyTo(sbools, 0);
            days.Add(sbools);

            bytes = s.Mon;
            bitArray = new BitArray(bytes);
            bool[] mbools = new bool[1440];
            bitArray.CopyTo(mbools, 0);
            days.Add(mbools);

            bytes = s.Tue;
            bitArray = new BitArray(bytes);
            bool[] tubools = new bool[1440];
            bitArray.CopyTo(tubools, 0);
            days.Add(tubools);

            bytes = s.Wed;
            bitArray = new BitArray(bytes);
            bool[] wbools = new bool[1440];
            bitArray.CopyTo(wbools, 0);
            days.Add(wbools);

            bytes = s.Thu;
            bitArray = new BitArray(bytes);
            bool[] thbools = new bool[1440];
            bitArray.CopyTo(thbools, 0);
            days.Add(thbools);

            bytes = s.Fre;
            bitArray = new BitArray(bytes);
            bool[] fbools = new bool[1440];
            bitArray.CopyTo(fbools, 0);
            days.Add(fbools);

            bytes = s.Sat;
            bitArray = new BitArray(bytes);
            bool[] sabools = new bool[1440];
            bitArray.CopyTo(sabools, 0);
            days.Add(sabools);


            weekPlan = days.ToImmutableArray();
            //}
            holidayLogic = container.Resolve<HolidayLogic>();
        }
 
 
        public DateTime GetEndDateTime(double processLength, DateTime start)
        {
            
            if(weekPlan.IsDefaultOrEmpty || processLength == 0) return start;
            bool[] tmpWeekPlan;
            int resultMinute = 0;
            double length = processLength;
            int startDay = (int)start.DayOfWeek;
            TimeSpan time = start.TimeOfDay;
            if (holidayLogic.IsHolyday(start)) { tmpWeekPlan = weekPlan[0]; } else tmpWeekPlan = weekPlan[startDay]; //set the start of the week
 
                for(int j = (int)time.TotalMinutes; j < tmpWeekPlan.Length; j++)
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
