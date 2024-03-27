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
            var r = (vorgang.Rstze == null) ? 0.0 : (short)vorgang.Rstze; //Setup time
            var c = (vorgang.Correction == null) ? 0.0 :(short)vorgang.Correction; //correction time
            if (vorgang.AidNavigation.Quantity != null && vorgang.AidNavigation.Quantity != 0.0) //if have Total quantity
            {
                //calculation of the currently required time
                var procT = (vorgang.Beaze == null) ? 0.0 : (short)vorgang.Beaze;
                var quant = (short)vorgang.AidNavigation.Quantity;
                var miss = (vorgang.QuantityMissNeo == null) ? 0.0 : (short)vorgang.QuantityMissNeo;
                var duration = procT / quant * miss + r + c;

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
            if (start.DayOfWeek == DayOfWeek.Saturday)
            {
                dateTime = dateTime.AddDays(2).Date;
                length = length.Add(TimeOnly.MaxValue.ToTimeSpan().Add(TimeOnly.MaxValue.ToTimeSpan()));
            }
            
            TimeSpan rest = timeSpan;

            var shifts = ressource.RessourceWorkshifts;
            if (shifts.Count == 0) { return TimeSpan.Zero; } //no shifts
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

                    var timespst = wsItem.StartTime.Value.ToTimeSpan();
                    var timespen = wsItem.EndTime.Value.ToTimeSpan();
                    if(length == TimeSpan.Zero)  //the first entry
                    {
                        if (dateTime.TimeOfDay < timespst) //we come from ground
                        {
                            length = length.Add(timespen.Subtract(dateTime.TimeOfDay));
                            rest -= timespen.Subtract(timespst);
                            if (rest.TotalMinutes < 0) //we are override
                            {
                                length = length.Add(rest);
                                break;
                            }
                        }
                        else if (dateTime.TimeOfDay < timespen) //we are between
                        {
                            length = length.Add(timespen.Subtract(dateTime.TimeOfDay));
                            rest -= timespen.Subtract(dateTime.TimeOfDay);
                            if (rest.TotalMinutes < 0) //we are override
                            {
                                length = length.Add(rest);
                                break;
                            }
                        }
                        else { continue; } //get next section
                    }
                    else if (dateTime.TimeOfDay < timespen) //we are between of further
                    {
                        length = length.Add(timespen.Subtract(endPos.ToTimeSpan()));
                        rest -= timespen.Subtract(timespst);
                        endPos = TimeOnly.FromTimeSpan(timespen);
                        if(rest.TotalMinutes < 0) //we are override
                        {
                            length = length.Add(rest);
                            break;
                        }
                    }
                                
                }
            }

            if (rest.TotalMinutes > 0) //we needs a next day?
            {
                length = length.Add(TimeOnly.MaxValue.ToTimeSpan().Subtract(endPos.ToTimeSpan()));
                length = GetCalculatedEndDate(rest, dateTime.AddDays(1).Date, ressource, length);
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
                if (string.IsNullOrEmpty(value) == false)
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
                if (string.IsNullOrEmpty(value) == false)
                    EndTime = TimeOnly.Parse(value);
                else EndTime = null;
                Changed |= true;
                NotifyPropertyChanged(() => EndTimeProxy);
            }
        }
    }
}
