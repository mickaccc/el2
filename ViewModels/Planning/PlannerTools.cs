using Lieferliste_WPF.Entities;
using Lieferliste_WPF.ViewModels;
using Lieferliste_WPF.Working;
using Lieferliste_WPF.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Lieferliste_WPF.Planning
{

    static class PlannerTools
    {

        private static double pCap;
        private static Collection<WorkingWeek> _Weeks;
        private static LinkedList<Process> Orders = new LinkedList<Process>();

        public static double? planningTest(double minute, DateUtils.CalendarWeek calendarWeek, Collection<WorkingWeek> Weeks)
        {
            _Weeks = Weeks;
            if (_Weeks.Count == 0) return null;
            DateUtils.CalendarWeek currentWeek = DateUtils.GetCalendarWeek(DateTime.Now);
            foreach (var week in _Weeks)
            {
                double free = week.getWorkingDays.Sum(y => y.getFreeCapacity());
                if (free >= minute)
                {
                    return free - minute;
                }
            }
            return null;
        }
        //private static DateTime? pTest(DateTime startDate, double min)
        //{
        //    DateTime? dt = null;
        //    DateTime testDay;
        //    try
        //    {

        //        var previousKvp = _Weeks.TakeWhile(x => x.Date != startDate)
        //            .LastOrDefault();

        //        if (previousKvp.Date == DateTime.MinValue)
        //        {
        //            return null;
        //        }

        //        testDay = previousKvp.Date;
        //        if (!(_Weeks.Any(x => x.Date==testDay))) { return null; }
        //        if (pCap != 0)
        //        {
        //            min -= _Weeks.Single(x => x.Date==testDay).getFreeCapacity();
        //        }
        //        pCap += _Weeks.Single(x => x.Date == testDay).getFreeCapacity();


        //        if (min > 0)
        //        {
        //            dt = pTest(testDay, min);
        //        }
        //        else
        //        {
        //            dt = startDate;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.AppendFormat(
        //            CultureInfo.CurrentCulture,
        //            "PlannerError {0}\n{1}\n{2}",
        //            "pTest",ex, ex.InnerException);
        //        _logger.Error(sb.ToString());
        //    }
        //    return dt;
        //}


        /// <summary>
        /// adds an Order at the begindate
        /// </summary>
        /// <param name="date"></param>
        /// <param name="order"></param>
        /// <param name="ref Orders"></param>
        /// <returns>ActionStripe</returns>
        public static void insertForce(Process order,
            Collection<WorkingWeek> workingWeeks)
        {
            _Weeks = workingWeeks;

            try
            {
                //LinkedListNode<Process> node = workingDays.AddLast(order);
                int cind = Orders.Count % 16;

                planningForce(order, cind);
                Orders.AddLast(order);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(
                    CultureInfo.CurrentCulture,
                    "PlannerError {0}\n{1}\n{2}",
                    "insertForce", ex, ex.InnerException);
            }
        }


        public static bool Reorganize(ref LinkedList<Process> workingLine)
        {
            foreach (WorkingWeek wek in _Weeks)
            {
                foreach (WorkingDayViewModel wd in wek.getWorkingDays)
                {
                    wd.ActionStripes.Clear();
                }
            }
            int cind = 0;
            foreach (Process order in workingLine)
            {
                planningForce(order, cind);
                cind++;
                //LinkedListNode<Process> p = Orders.Find(order);
                //Process pro = (p.Previous!=null) ? p.Previous.Value: null;


                //int preStart = Days.First(x => x.Date==endT.Date).getWorkingMinutes()[0];

                //if (pro != null) preStart = Days.Last(x => x.ActionStripes.Any(y => y.Name==pro.VID)).ActionStripes.FindLast(c => c.Name==pro.VID).End+1;
                //order.ActionStripes.First().Start = preStart;
                //order.ActionStripes.First().Date = endT.Date;
                //endT = planningForce(order.ActionStripes);
            }
            return false;
        }

        private static DateTime planningForce(Process order, int colorIndex)
        {
            double sumL = order.ProcessRestTime;
            WorkingDayViewModel wDay = null;
            while (sumL > 0)
            {
                var result = (from w in _Weeks
                              from d in w.getWorkingDays
                              select d).FirstOrDefault(x => x.getFreeCapacity() > 0);

                if (result == null)
                {
                    WorkingWeek w = new WorkingWeek(DateUtils.GetGermanCalendarWeek(_Weeks.Last().Monday.Date.AddDays(7)));
                    _Weeks.Add(w);
                    wDay = w.getWorkingDays[0];
                }

                wDay = result;
                ActionStripe ac = new ActionStripe(order.OrderNumber, order.ProcessTime, wDay.Date, order.deadKW.Week)
                {
                    VNR = order.ExecutionNumber,
                    TTNR = order.Material,
                    ColorIndex = colorIndex
                };
                if (wDay.ActionStripes.Count > 0)
                {
                    ac.Start = wDay.ActionStripes.Last().End;
                }
                else
                {
                    ac.Start = wDay.getWorkingMinutes().ElementAt(0);
                }

                if (wDay.getFreeCapacity() > sumL)
                {
                    int v = Convert.ToInt32(wDay.getWorkingMinutes().TakeWhile(y => y.CompareTo(ac.Start) < 0).Count() + sumL - 1);
                    if (v < 0)
                    {
                        v++;
                    }

                    ac.CalcLenght = Convert.ToInt32(sumL);
                    ac.End = wDay.getWorkingMinutes().ElementAt(v);

                    sumL = 0;
                }
                else
                {
                    ac.CalcLenght = (wDay.getFreeCapacity());
                    int ind = wDay.getWorkingMinutes().TakeWhile(y => y.CompareTo(ac.Start) < 0).Count() + ac.CalcLenght - 1;
                    ac.End = wDay.getWorkingMinutes().ElementAt(ind);
                    sumL -= ac.CalcLenght;
                }
                wDay.ActionStripes.Add(ac);
            }
            ActionStripe ret = wDay.ActionStripes.Last();
            return new DateTime(ret.Date.Date.Ticks + ret.EndTime.Ticks);
        }

        public static bool moveForce(Process targetBefore, Process sourceOrder, ref LinkedList<Process> orders)
        {

            if (targetBefore.Equals(sourceOrder)) return false;

            LinkedListNode<Process> node = orders.Find(sourceOrder);
            LinkedListNode<Process> current = orders.Find(targetBefore);
            //node.Value.ActionStripes.First().CalcLenght = 0;
            //current.Value.ActionStripes.First().CalcLenght = 0;
            int indexNode = orders.TakeWhile(x => !x.Equals(sourceOrder)).Count();
            int indexCurrent = orders.TakeWhile(x => !x.Equals(targetBefore)).Count();

            //DateTime dt;
            if (indexNode > indexCurrent)
            {
                orders.Remove(node);
                //    dt = current.Value.ActionStripes.First().Date;
                orders.AddBefore(current, node);
            }
            else
            {
                orders.Remove(node);
                //    dt = node.Value.ActionStripes.First().Date;
                orders.AddAfter(current, node);
            }
            return Reorganize(ref orders);
        }

    }
}
