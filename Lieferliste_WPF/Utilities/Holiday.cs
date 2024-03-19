using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;


namespace Lieferliste_WPF.Utilities
{
    public class Holiday : IComparable<Holiday>
    {
        private int type;
        private DateTime datum;
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
        public DateTime Datum
        {
            get => datum;
            set => datum = value;
        }


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
            return this.datum.CompareTo(other.datum);
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
            
                var easter = GetEasterSunday();
                foreach(var d in CloseAndHolidayRule.FixHoliday)
                {
                    var holi = new Holiday(new DateTime(CurrentYear, d.month, d.day), d.name, 1, Locale);
                    holydays.Add(DateOnly.FromDateTime(holi.Datum), holi);
                }
                foreach(var d in CloseAndHolidayRule.VariousHoliday)
                {
                    var holi = new Holiday(easter.AddDays(d.dayDistance), d.name, 2, Locale);
                    holydays.Add(DateOnly.FromDateTime(holi.Datum), holi);
                }
                foreach(var d in CloseAndHolidayRule.CloseDay)
                {
                    holydays.Add(DateOnly.FromDateTime(d.Datum), d);
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

        //public string SaveHolidays()
        //{
        //    //var ch = new CloseAndHolidayRule();


        //    //var xml = XmlSerializerHelper.GetSerializer(typeof(CloseAndHolidayRule));
        //    //StringWriter sw = new ();
        //    ////xml.Serialize(sw, CloseAndHolidayRule);
        //    //ImmutableArray<List<Holiday>> arrVar = [];
        //    //arrVar.Add(ch.VariousHoliday);
        //    //arrVar.Add(ch.FixHoliday);
        //    //xml.Serialize(sw, ch);
            
        //    //return sw.ToString();
        //}
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
    [Serializable]
    public static class CloseAndHolidayRule
    {
        public static readonly List<HolidayRule> FixHoliday = new();
        public static readonly List<HolidayRule> VariousHoliday = new();
        public static readonly List<Holiday> CloseDay = new();


        //VariousHoliday.Add(new Holiday(1, "Ostermontag", "de-AT"));
        //VariousHoliday.Add(new Holiday(39, "Christi Himmelfahrt", "de-AT"));
        //VariousHoliday.Add(new Holiday(49, "Pfingstsonntag", "de-AT"));
        //VariousHoliday.Add(new Holiday(50, "Pfingstmontag", "de-AT"));
        //VariousHoliday.Add(new Holiday(60, "Fronleichnam", "de-AT"));

        //FixHoliday.Add(new Holiday(1, 1, "Neujahr", "de-AT"));
        //FixHoliday.Add(new Holiday(6, 1, "Heilige Drei Könige", "de-AT"));
        //FixHoliday.Add(new Holiday(1, 5, "Staatsfeiertag", "de-AT"));
        //FixHoliday.Add(new Holiday(15, 8, "Mariä Himmelfahrt", "de-AT"));
        //FixHoliday.Add(new Holiday(26, 10, "Nationalfeiertag", "de-AT"));
        //FixHoliday.Add(new Holiday(25, 12, "Christtag", "de-AT"));
        //FixHoliday.Add(new Holiday(26, 12, "Stefanitag", "de-AT"));

        public class HolidayRule
        {
            public string name = string.Empty;
            public int day;
            public int month;
            public double dayDistance;
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
}

