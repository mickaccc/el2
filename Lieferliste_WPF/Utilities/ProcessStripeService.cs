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
        public static DateTime GetProcessLength(Vorgang vorgang, DateTime start, out TimeSpan ProcessLength)
        {
            var r = vorgang.Rstze ?? 0;
            var c = vorgang.Correction ?? 0;
            if (vorgang.AidNavigation.Quantity != null && vorgang.AidNavigation.Quantity != 0)
            {         
                var duration = vorgang.Beaze ?? 0 / vorgang.AidNavigation.Quantity * vorgang.QuantityMissNeo ?? 0 + r + c;
                duration = 100;
                if (duration > 0)
                {
                    TimeSpan t = TimeSpan.FromMinutes(Convert.ToDouble(duration));
                    ProcessLength = GetCalculatedEndDate(t, start, vorgang.RidNavigation, TimeSpan.Zero);
                    return start.AddTicks(ProcessLength.Ticks);
                }
            }
            ProcessLength = TimeSpan.Zero;
            return start;
        }
        private static TimeSpan GetCalculatedEndDate(TimeSpan timeSpan, DateTime start, Ressource ressource, TimeSpan length)
        {
            DateTime dateTime = start;
            if(start.DayOfWeek == DayOfWeek.Saturday) dateTime = dateTime.AddDays(1).Date;
            
            TimeSpan rest = timeSpan;

            var shifts = ressource.RessourceWorkshifts;
            if (shifts.Count == 0) { return TimeSpan.Zero; }
            var serializer = XmlSerializerHelper.GetSerializer(typeof(List<WorkShiftItem>));
            TimeOnly endPos = TimeOnly.FromDateTime(dateTime);
            foreach (var shift in shifts)
            {
                while (HolydayLogic.GetInstance(dateTime.Year).isHolyday(dateTime)) { dateTime = dateTime.AddDays(1).Date; }
                if (rest.TotalMinutes < 0) break;
                var data = shift.SidNavigation.ShiftDef;
                TextReader reader = new StringReader(data);
                var ws = (List<WorkShiftItem>)serializer.Deserialize(reader);
 
                foreach (var wsItem in ws)
                {
                    DateTime endDate;
                    var timespst = wsItem.StartTime.Value.ToTimeSpan();
                    var timespen = wsItem.EndTime.Value.ToTimeSpan();
                    if(length == TimeSpan.Zero)
                    {
                        if (dateTime.TimeOfDay < timespst)
                        {
                            length = length.Add(timespen.Subtract(dateTime.TimeOfDay));
                            rest -= timespen.Subtract(timespst);
                        }
                        else if (dateTime.TimeOfDay < timespen)
                        {
                            length = length.Subtract(timespen.Subtract(dateTime.TimeOfDay));
                            rest -= timespen.Subtract(dateTime.TimeOfDay);
                        }
                        else { continue; }
                    }
                    else if (dateTime.TimeOfDay < timespen)
                    {
                        length = length.Add(timespen.Subtract(endPos.ToTimeSpan()));
                        rest -= timespen.Subtract(timespst);
                        endPos = TimeOnly.FromTimeSpan(timespen);
                    }
                                
                }
            }

            if (rest.TotalMinutes > 0)
            {

                length = length.Add(GetCalculatedEndDate(rest, dateTime.AddDays(1).Date, ressource,
                    length.Add(TimeOnly.MaxValue.ToTimeSpan().Subtract(endPos.ToTimeSpan()))) );
            }
            return length;
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
