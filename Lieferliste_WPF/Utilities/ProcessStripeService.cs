using El2Core.Models;
using El2Core.ViewModelBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace Lieferliste_WPF.Utilities
{
    internal static class ProcessStripeService
    {
        public static TimeSpan GetProcessLength(Vorgang vorgang, DateTime start, out DateTime endTime)
        {
            var r = vorgang.Rstze ?? 0;
            var c = vorgang.Correction ?? 0;
            var result = vorgang.Beaze / vorgang.AidNavigation.Quantity * vorgang.QuantityMiss + r + c;

            TimeSpan t = TimeSpan.FromMinutes(Convert.ToDouble(result));
            endTime = GetCalculatedEndDate(t, start, vorgang.RidNavigation);
            return TimeSpan.FromTicks(endTime.Ticks - start.Ticks);
        }
        private static DateTime GetCalculatedEndDate(TimeSpan timeSpan, DateTime start, Ressource ressource)
        {
            DateTime dateTime = start;
            if(start.DayOfWeek == DayOfWeek.Saturday) dateTime = dateTime.AddDays(2).Date;

            TimeSpan rest = timeSpan;
            DateTime result = start;
            var shifts = ressource.RessourceWorkshifts;
            if (shifts.Count == 0) { return start; }
            var serializer = XmlSerializerHelper.GetSerializer(typeof(List<WorkShiftItem>));
            foreach (var shift in shifts)
            {
                var data = shift.SidNavigation.ShiftDef;
                TextReader reader = new StringReader(data);
                var ws = (List<WorkShiftItem>)serializer.Deserialize(reader);

                foreach (var wsItem in ws)
                {
                    var timespst = wsItem.StartTime.Value.ToTimeSpan();
                    var timespen = wsItem.EndTime.Value.ToTimeSpan();
                    while (HolydayLogic.GetInstance(dateTime.Year).isHolyday(dateTime)) { dateTime = dateTime.AddDays(1).Date; }
                    if (dateTime.TimeOfDay < timespst) dateTime = dateTime.Date.AddTicks(timespst.Ticks);
                    else if (dateTime.TimeOfDay > timespen) dateTime = dateTime.Date.AddTicks(timespen.Ticks);
                    else { continue; }
                    var restDate = dateTime.AddTicks(rest.Ticks);
                    rest = TimeSpan.FromTicks(restDate.Subtract(dateTime.Date.AddTicks(timespen.Ticks)).Ticks);
                    if (rest.Ticks > 0) { dateTime = dateTime.Date.AddTicks(timespen.Ticks); }
                    else { result = dateTime.AddTicks(rest.Ticks); }
                }
            }

            if (rest.TotalMinutes > 0)
            {
                result = GetCalculatedEndDate(rest, dateTime.AddDays(1).Date, ressource);
            }
            return result;
        }
 
    }
    public class WorkShiftService
    {
        public WorkShiftService() { }
        private int _id;
        public int id
        {
            get { return _id; }
            set
            {
                _id = value;
                Changed = true;
            }
        }
        public bool Changed { get; set; } = false;
        public string ShiftName { get; set; }
        public ObservableCollection<WorkShiftItem> Items { get; set; } = [];
    }
    public class WorkShiftItem : ViewModelBase
    { 
        public WorkShiftItem() { }
        [XmlIgnore]
        public TimeOnly? StartTime { get; set; }

        [XmlIgnore]
        public TimeOnly? EndTime { get; set; }
        [XmlIgnore]
        public bool Changed { get; set; } = false;
        public string? StartTimeProxy
        {
            get { return StartTime.ToString(); }
            set
            {
                if (value != null)
                    StartTime = TimeOnly.Parse(value);
                else StartTime = null;
                Changed = true;
                NotifyPropertyChanged(()  => StartTimeProxy);
            }
        }
        public string? EndTimeProxy
        {
            get { return EndTime.ToString(); }
            set
            {
                if (value != null)
                    EndTime = TimeOnly.Parse(value);
                else EndTime = null;
                Changed |= true;
                NotifyPropertyChanged(() => EndTimeProxy);
            }
        }
    }
}
