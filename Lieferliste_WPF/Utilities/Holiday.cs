using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Xml.Serialization;


namespace Lieferliste_WPF.Utilities
{
    public class Holiday : IComparable<Holiday>
    {
        private int type;
        private int? timeDistance;
        private int? month;
        private string _locale = "";

        public Holiday() { }
        public Holiday(DateTime datum, string name, int type, string locale)
        {
            this.type = type;
            this.Datum = datum;
            this.Name = name;
            this._locale = locale;
        }

        /// <summary>
        /// Beschreibung: 
        /// </summary>
        public string Name { get; set; } = "";


        /// <summary>
        /// Beschreibung: 
        /// </summary>
        public DateTime Datum { get; set; }   


        /// <summary>
        /// Beschreibung: 
        /// </summary>
        public int Type => type;
  
        /// <summary>
        /// Locale String
        /// </summary>
        public string  Locale => _locale;


        #region IComparable<Holyday> Member

        public int CompareTo(Holiday other)
        {
            return this.Datum.CompareTo(other.Datum);
        }

        #endregion
    }
    public class HolydayLogic
    {
        private static HolydayLogic? Instance;
        private Dictionary<DateOnly, Holiday> holydays;
        private int year;
        private String? locale;

        public Dictionary<DateOnly, Holiday> Holidays { get { return holydays; } }
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
        public static HolydayLogic GetInstance(int year, String locale = "")
        {
            if (Instance == null || year != Instance.CurrentYear)
            {
                Instance = new HolydayLogic(year);
                if (locale == "") { Instance.Locale = locale; } else { Instance.Locale = Thread.CurrentThread.CurrentCulture.Name; }
                return Instance;
            }
            return Instance;
        }

        public bool isHolyday(DateTime value)
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

        private HolydayLogic(int year)
        {
            this.CurrentYear = year;
            #region fillList
            this.holydays = new();
            CloseAndHolidayRule holiRule = new CloseAndHolidayRule();
                var easter = GetEasterSunday();
            if (holiRule.FixHoliday != null)
            {
                foreach (var d in holiRule.FixHoliday)
                {
                    var holi = new Holiday(new DateTime(CurrentYear, (int)d.Month, (int)d.Day), d.Name, 1, d.Locale);
                    holydays.Add(DateOnly.FromDateTime(holi.Datum), holi);
                } 
            }
            if (holiRule.VariousHoliday != null)
            {
                foreach (var d in holiRule.VariousHoliday)
                {
                    var holi = new Holiday(easter.AddDays(Convert.ToDouble(d.DayDistance)), d.Name, 2, d.Locale);
                    holydays.Add(DateOnly.FromDateTime(holi.Datum), holi);
                } 
            }
            if (holiRule.CloseDay != null)
            {
                foreach (var d in holiRule.CloseDay)
                {
                    holydays.Add(DateOnly.FromDateTime(d.Datum), d);
                }
            }
            //    new Holiday(1,1, "Neujahr", "de-DE"),
            //    new Holiday(6,1, "Heilige Drei Könige", "de-DE"),
            //    new Holiday(1,5, "Tag der Arbeit", "de-DE"),
            //    new Holiday(15, 8, "Mariä Himmelfahrt", "de-DE"),
            //    new Holiday(3, 10, "Tag der dt. Einheit", "de-DE"),
            //    new Holiday(31, 10, "Reformationstag", "de-DE"),
            //    new Holiday(1, 11, "Allerheiligen", "de-DE"),
            //    new Holiday(25, 12, "1. Weihnachtstag", "de-DE"),
            //    new Holiday(26, 12, "2. Weihnachtstag", "de-DE")
            //};
            //var osterSonntag = GetEasterSunday();
            ////this.holydays.Add(new Holyday(false, osterSonntag, "Ostersonntag", "de-DE"));
            //this.holydays.Add(new Holiday(-3, "Gründonnerstag", "de-DE"));
            //this.holydays.Add(new Holiday(-2, "Karfreitag", "de-DE"));
            //this.holydays.Add(new Holiday(1, "Ostermontag", "de-DE"));
            //this.holydays.Add(new Holiday(39, "Christi Himmelfahrt", "de-DE"));
            //this.holydays.Add(new Holiday(49, "Pfingstsonntag", "de-DE"));
            //this.holydays.Add(new Holiday(50, "Pfingstmontag", "de-DE"));
            //this.holydays.Add(new Holiday(60, "Fronleichnam", "de-DE"));

            //this.holydays.Add(new Holiday(1, 1, "Neujahr", "de-AT"));
            //this.holydays.Add(new Holiday(6, 1, "Heilige Drei Könige", "de-AT"));
            //this.holydays.Add(new Holiday(1, 5, "Staatsfeiertag", "de-AT"));
            //this.holydays.Add(new Holiday(15, 8, "Mariä Himmelfahrt", "de-AT"));
            //this.holydays.Add(new Holiday(26, 10, "Nationalfeiertag", "de-AT"));
            //this.holydays.Add(new Holiday(1, 11, "Allerheiligen", "de-AT"));
            //this.holydays.Add(new Holiday(25, 12, "Christtag", "de-AT"));
            //this.holydays.Add(new Holiday(26, 12, "Stefanitag", "de-AT"));

            ////this.holydays.Add(new Holyday(false, osterSonntag, "Ostersonntag", "de-AT"));
            //this.holydays.Add(new Holiday(1, "Ostermontag", "de-AT"));
            //this.holydays.Add(new Holiday(39, "Christi Himmelfahrt", "de-AT"));
            //this.holydays.Add(new Holiday(49, "Pfingstsonntag", "de-AT"));
            //this.holydays.Add(new Holiday(50, "Pfingstmontag", "de-AT"));
            //this.holydays.Add(new Holiday(60, "Fronleichnam", "de-AT"));

            //this.holydays.RemoveAll(x => x.Locale != this.locale);

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

        public readonly List<HolidayRule>? FixHoliday  = [];
        public readonly List<HolidayRule>? VariousHoliday  = [];
        public readonly List<Holiday>? CloseDay;

        public CloseAndHolidayRule()
        {
            //VariousHoliday.Add(new HolidayRule() { DayDistance = 1, Name = "Ostermontag", Locale = "de-AT" });
            //VariousHoliday.Add(new HolidayRule() { DayDistance = 39, Name = "Christi Himmelfahrt", Locale = "de-AT" });
            //VariousHoliday.Add(new HolidayRule() { DayDistance = 49, Name = "Pfingstsonntag", Locale = "de-AT" });
            //VariousHoliday.Add(new HolidayRule() { DayDistance = 50, Name = "Pfingstmontag", Locale = "de-AT" });
            //VariousHoliday.Add(new HolidayRule() { DayDistance = 60, Name = "Fronleichnam", Locale = "de-AT" });

            //FixHoliday.Add(new HolidayRule() { Day = 1, Month = 1, Name = "Neujahr", Locale = "de-AT" });
            //FixHoliday.Add(new HolidayRule() { Day = 6, Month = 1, Name = "Heilige Drei Könige", Locale = "de-AT" });
            //FixHoliday.Add(new HolidayRule() { Day = 1, Month = 5, Name = "Staatsfeiertag", Locale = "de-AT" });
            //FixHoliday.Add(new HolidayRule() { Day = 15, Month = 8, Name = "Mariä Himmelfahrt", Locale = "de-AT" });
            //FixHoliday.Add(new HolidayRule() { Day = 26, Month = 10, Name = "Nationalfeiertag", Locale = "de-AT" });
            //FixHoliday.Add(new HolidayRule() { Day = 25, Month = 12, Name = "Christtag", Locale = "de-AT" });
            //FixHoliday.Add(new HolidayRule(){ Day=26, Month= 12, Name="Stefanitag", Locale="de-AT"});
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

    public static class XmlSerializerHelper
    {
        private static readonly ConcurrentDictionary<Type, XmlSerializer> TypeSerializers = new ConcurrentDictionary<Type, XmlSerializer>();

        public static XmlSerializer GetSerializer(Type type)
        {
            return TypeSerializers.GetOrAdd(type,
            t =>
            {
                var importer = new XmlReflectionImporter();
                var mapping = importer.ImportTypeMapping(t, null, null);
                return new XmlSerializer(mapping);
            });
        }
    }


    //// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    ///// <remarks/>
    //[System.SerializableAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    //public partial class CloseAndHolidayRule
    //{

    //    private CloseAndHolidayRuleHolidayRule[] fixHolidayField;

    //    private CloseAndHolidayRuleHolidayRule1[] variousHolidayField;

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("HolidayRule", IsNullable = false)]
    //    public CloseAndHolidayRuleHolidayRule[] FixHoliday
    //    {
    //        get
    //        {
    //            return this.fixHolidayField;
    //        }
    //        set
    //        {
    //            this.fixHolidayField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlArrayItemAttribute("HolidayRule", IsNullable = false)]
    //    public CloseAndHolidayRuleHolidayRule1[] VariousHoliday
    //    {
    //        get
    //        {
    //            return this.variousHolidayField;
    //        }
    //        set
    //        {
    //            this.variousHolidayField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.SerializableAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    //public partial class CloseAndHolidayRuleHolidayRule
    //{

    //    private string nameField;

    //    private string localeField;

    //    private byte dayField;

    //    private byte monthField;

    //    private object dayDistanceField;

    //    /// <remarks/>
    //    public string Name
    //    {
    //        get
    //        {
    //            return this.nameField;
    //        }
    //        set
    //        {
    //            this.nameField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string Locale
    //    {
    //        get
    //        {
    //            return this.localeField;
    //        }
    //        set
    //        {
    //            this.localeField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public byte Day
    //    {
    //        get
    //        {
    //            return this.dayField;
    //        }
    //        set
    //        {
    //            this.dayField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public byte Month
    //    {
    //        get
    //        {
    //            return this.monthField;
    //        }
    //        set
    //        {
    //            this.monthField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
    //    public object DayDistance
    //    {
    //        get
    //        {
    //            return this.dayDistanceField;
    //        }
    //        set
    //        {
    //            this.dayDistanceField = value;
    //        }
    //    }
    //}

    ///// <remarks/>
    //[System.SerializableAttribute()]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    //public partial class CloseAndHolidayRuleHolidayRule1
    //{

    //    private string nameField;

    //    private string localeField;

    //    private object dayField;

    //    private object monthField;

    //    private byte dayDistanceField;

    //    /// <remarks/>
    //    public string Name
    //    {
    //        get
    //        {
    //            return this.nameField;
    //        }
    //        set
    //        {
    //            this.nameField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string Locale
    //    {
    //        get
    //        {
    //            return this.localeField;
    //        }
    //        set
    //        {
    //            this.localeField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
    //    public object Day
    //    {
    //        get
    //        {
    //            return this.dayField;
    //        }
    //        set
    //        {
    //            this.dayField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
    //    public object Month
    //    {
    //        get
    //        {
    //            return this.monthField;
    //        }
    //        set
    //        {
    //            this.monthField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public byte DayDistance
    //    {
    //        get
    //        {
    //            return this.dayDistanceField;
    //        }
    //        set
    //        {
    //            this.dayDistanceField = value;
    //        }
    //    }
    //}



}

