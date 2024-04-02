﻿using El2Core.Models;
using El2Core.Utils;
using El2Core.ViewModelBase;
using Prism.Ioc;
using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;


namespace Lieferliste_WPF.Utilities
{

    public class Holiday : ViewModelBase, IComparable<Holiday>
    {
        private string _locale = "";
        public Holiday() { }
        public Holiday(DateTime datum, string name, int type, string locale)
        {
            this.type = type;
            this.datum = datum;
            this.name = name;
            this._locale = locale;
        }

        /// <summary>
        /// Beschreibung: 
        /// </summary>
        private string? name;
        public string? Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged(() => Name);
                }
            }
        }


        /// <summary>
        /// Beschreibung: 
        /// </summary>
        private DateTime datum = DateTime.Now;
        public DateTime Datum
        {
            get { return datum; }
            set
            {
                if (value != datum)
                {
                    datum = value;
                    NotifyPropertyChanged(() => Datum);
                }
            }
        }


        /// <summary>
        /// Beschreibung: 
        /// </summary>
        private int type;
        public int Type
        {
            get { return type; }
            set
            {
                if (value != type)
                {
                    type = value;
                    NotifyPropertyChanged(() => Type);
                }
            }
        }
  
        /// <summary>
        /// Locale String
        /// </summary>
        public string  Locale => _locale;


        #region IComparable<Holyday> Member

        public int CompareTo(Holiday? other)
        {
            return this.datum.CompareTo(other?.datum);
        }

        #endregion
    }
    public interface IHolidayLogic { }
    public class HolidayLogic : IHolidayLogic 
    {
        IContainerExtension _container;
        private FrozenDictionary<DateOnly, Holiday> holydays;
        private int year;
        private String? locale;

        internal FrozenDictionary<DateOnly, Holiday> Holidays { get { return holydays; } }
        /// <summary>
        /// Beschreibung: 
        /// </summary>
        public int CurrentYear
        {
            get { return year; }
            set { year = value; }
        }
        public String? Locale
        {
            get { return locale; }
            set { locale = value; }
        }

        public bool IsHolyday(DateTime value)
        {
            return holydays.ContainsKey(DateOnly.FromDateTime(value));
        }

        public string GetHolydayName(DateTime Datevalue)
        {
            var d = DateOnly.FromDateTime(Datevalue);
            if(holydays.TryGetValue(d, out Holiday? value))
                return value.Name;
            return "no Holiday";
        }

        public HolidayLogic(IContainerExtension container)
        {
            _container = container;
            this.year = DateTime.Now.Year;
            #region fillList
            Dictionary<DateOnly, Holiday> dict = new();

            
            using var db = _container.Resolve<DB_COS_LIEFERLISTE_SQLContext>();
            var holis = db.Rules.Find(3);
            StringReader reader = new StringReader(holis.RuleData);
            var serializer = XmlSerializerHelper.GetSerializer(typeof(CloseAndHolidayRule));
            var holiRule = (CloseAndHolidayRule)serializer.Deserialize(reader);
            for (int i = year; i <= year + 1; i++)
            {
                var easter = getGaussianEaster(i);
                if (holiRule.FixHoliday != null)
                {
                    foreach (var d in holiRule.FixHoliday)
                    {
                        var holi = new Holiday(new DateTime(i, (int)d.Month, (int)d.Day), d.Name, 1, d.Locale);
                        dict.Add(DateOnly.FromDateTime(holi.Datum), holi);
                    }
                }
                if (holiRule.VariousHoliday != null)
                {
                    foreach (var d in holiRule.VariousHoliday)
                    {
                        var holi = new Holiday(easter.AddDays(Convert.ToDouble(d.DayDistance)), d.Name, 2, d.Locale);
                        dict.Add(DateOnly.FromDateTime(holi.Datum), holi);
                    }
                }
                if (holiRule.CloseDay != null)
                {
                    foreach (var d in holiRule.CloseDay)
                    {
                        if(d.Datum.Year == i)
                            dict.TryAdd(DateOnly.FromDateTime(d.Datum), d);
                    }
                }
            }
            holydays = dict.ToFrozenDictionary();

            #endregion
        }

        private DateTime GetEasterSunday()
        {
            var g = year % 19;
            var c = this.year / 100;
            var h = ((c - (c / 4)) - (((8 * c) + 13) / 25) + (19 * g) + 15) % 30;
            var i = h - (h / 28) * (1 - (29 / (h + 1)) * ((21 - g) / 11));
            var j = (year + (year / 4) + i + 2 - c + (c / 4)) % 7;

            var l = i - j;
            var month = (int)(3 + ((l + 40) / 44));
            var day = (int)(l + 28 - 31 * (month / 4));

            return new DateTime(year, month, day);

        }
        private static DateTime getGaussianEaster(int year)
        {

            var k = year / 100;
            var tmp = (3 * k + 3) / 4;
            var m = 15 + tmp - (8 * k + 13) / 25;
            var s = 2 - tmp;
            var a = year % 19;
            var d = (19 * a + m) % 30;
            var r = (d / 29) + (d / 28 - d / 29) * (a / 11);
            var og = 21 + d - r;
            var sz = 7 - (year + year / 4 + s) % 7;
            var oe = 7 - (og - sz) % 7;
            var os = og + oe - 1;

            var cal = new DateTime(year, 3, 1);

            cal.AddDays(os);

            return (cal);
        }
    }
  
    public class CloseAndHolidayRule
    {

        public readonly List<HolidayRule> FixHoliday  = [];
        public readonly List<HolidayRule> VariousHoliday  = [];
        public readonly List<Holiday> CloseDay = [];

        public CloseAndHolidayRule()
        {
        }
        
    }

    public class HolidayRule
    {
        public HolidayRule() { }
        private string name = string.Empty;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string locale = string.Empty;

        public string Locale
        {
            get { return locale; }
            set { locale = value; }
        }
        private int? day;

        public int? Day
        {
            get { return day; }
            set { day = value; }
        }
        private int? month;

        public int? Month
        {
            get { return month; }
            set { month = value; }
        }

        private int? dayDistance;

        public int? DayDistance
        {
            get { return dayDistance; }
            set { dayDistance = value; }
        }

    }
    public interface IApprover
    {
        IApprover SetNext(IApprover approver);
        bool IsHoliday(DateTime date);
    }

    /// <summary>
    /// The 'Handler' abstract class
    /// </summary>
    public abstract class Approver : IApprover
    {

        public IApprover SetNext(IApprover approver)
        {
            this.Successor = approver;
            return approver;
        }

        public virtual bool IsHoliday(DateTime date)
        {
            return false;
        }

        // Sets or gets the next approver
        public IApprover Successor { get; set; } = null!;
    }
}

