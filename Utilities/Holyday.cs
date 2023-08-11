﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace Lieferliste_WPF.Utilities
{
    public class Holyday : IComparable<Holyday>
    {
        private bool isFix;
        private DateTime datum;
        private string name = "";
        private string _locale = "";

        public Holyday(bool isFix, DateTime datum, string name, string locale)
        {
            this.IsFix = isFix;
            this.Datum = datum;
            this.Name = name;
            this._locale = locale;

        }

        /// <summary>
        /// Beschreibung: 
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        /// <summary>
        /// Beschreibung: 
        /// </summary>
        public DateTime Datum
        {
            get { return datum; }
            set { datum = value; }
        }


        /// <summary>
        /// Beschreibung: 
        /// </summary>
        public bool IsFix
        {
            get { return isFix; }
            set { isFix = value; }
        }
        /// <summary>
        /// Locale String
        /// </summary>
        public string Locale
        {
            get { return _locale; }
            set { _locale = value; }
        }

        #region IComparable<Holyday> Member

        public int CompareTo(Holyday other)
        {
            return this.datum.Date.CompareTo(other.datum.Date);
        }

        #endregion
    }
    public class HolydayLogic
    {
        private static HolydayLogic Instance;
        private List<Holyday> holydays;
        private int year;
        private String locale;

        /// <summary>
        /// Beschreibung: 
        /// </summary>
        public int CurrentYear
        {
            get { return year; }
            set { year = value;}
        }
        public String Locale
        {
            get { return locale; }
            set {  locale = value; }
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

        /// <summary>
        /// Beschreibung: Gibt variable Feiertage zurueck
        /// </summary>
        public List<Holyday> VariousHolydays
        {
            get
            {
                return holydays.FindAll(delegate (Holyday f) { return !f.IsFix; });
            }

        }

        public bool isHolyday(DateTime value)
        {
            return (holydays.Find(delegate (Holyday f) { return f.Datum.Date == value.Date; }) != null);
        }

        public Holyday GetHolydayName(DateTime value)
        {
            return (holydays.Find(delegate (Holyday f) { return f.Datum.Date == value.Date; }));
        }
        /// <summary>
        /// Beschreibung: gibt feste Feiertage zurueck
        /// </summary>
        public List<Holyday> FixHolydays
        {
            get
            {
                return holydays.FindAll(delegate (Holyday f) { return f.IsFix; });
            }
        }

        private HolydayLogic(int year)
        {
            this.CurrentYear = year;
            #region fillList
            this.holydays = new List<Holyday>();
            this.holydays.Add(new Holyday(true, new DateTime(year, 1, 1), "Neujahr","de-DE"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 1, 6), "Heilige Drei Könige","de-DE"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 5, 1), "Tag der Arbeit", "de-DE"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 8, 15), "Mariä Himmelfahrt", "de-DE"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 10, 3), "Tag der dt. Einheit", "de-DE"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 10, 31), "Reformationstag", "de-DE"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 11, 1), "Allerheiligen", "de-DE"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 12, 25), "1. Weihnachtstag", "de-DE"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 12, 26), "2. Weihnachtstag", "de-DE"));
            DateTime osterSonntag = GetEasterSunday();
            this.holydays.Add(new Holyday(false, osterSonntag, "Ostersonntag", "de-DE"));
            this.holydays.Add(new Holyday(false, osterSonntag.AddDays(-3), "Gründonnerstag", "de-DE"));
            this.holydays.Add(new Holyday(false, osterSonntag.AddDays(-2), "Karfreitag", "de-DE"));
            this.holydays.Add(new Holyday(false, osterSonntag.AddDays(1), "Ostermontag", "de-DE"));
            this.holydays.Add(new Holyday(false, osterSonntag.AddDays(39), "Christi Himmelfahrt", "de-DE"));
            this.holydays.Add(new Holyday(false, osterSonntag.AddDays(49), "Pfingstsonntag", "de-DE"));
            this.holydays.Add(new Holyday(false, osterSonntag.AddDays(50), "Pfingstmontag", "de-DE"));
            this.holydays.Add(new Holyday(false, osterSonntag.AddDays(60), "Fronleichnam", "de-DE"));

            this.holydays.Add(new Holyday(true, new DateTime(year, 1, 1), "Neujahr", "de-AT"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 1, 6), "Heilige Drei Könige", "de-AT"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 5, 1), "Staatsfeiertag", "de-AT"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 8, 15), "Mariä Himmelfahrt", "de-AT"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 10, 26), "Nationalfeiertag", "de-AT"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 11, 1), "Allerheiligen", "de-AT"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 12, 25), "Christtag", "de-AT"));
            this.holydays.Add(new Holyday(true, new DateTime(year, 12, 26), "Stefanitag", "de-AT"));

            this.holydays.Add(new Holyday(false, osterSonntag, "Ostersonntag", "de-AT"));
            this.holydays.Add(new Holyday(false, osterSonntag.AddDays(1), "Ostermontag", "de-AT"));
            this.holydays.Add(new Holyday(false, osterSonntag.AddDays(39), "Christi Himmelfahrt", "de-AT"));
            this.holydays.Add(new Holyday(false, osterSonntag.AddDays(49), "Pfingstsonntag", "de-AT"));
            this.holydays.Add(new Holyday(false, osterSonntag.AddDays(50), "Pfingstmontag", "de-AT"));
            this.holydays.Add(new Holyday(false, osterSonntag.AddDays(60), "Fronleichnam", "de-AT"));

            this.holydays.RemoveAll(x => x.Locale != this.locale);

            #endregion
        }

        private DateTime GetEasterSunday()
        {

            int g, h, c, j, l, i;

            g = year % 19;
            c = this.year / 100;
            h = ((c - (c / 4)) - (((8 * c) + 13) / 25) + (19 * g) + 15) % 30;
            i = h - (h / 28) * (1 - (29 / (h + 1)) * ((21 - g) / 11));
            j = (year + (year / 4) + i + 2 - c + (c / 4)) % 7;

            l = i - j;
            int month = (int)(3 + ((l + 40) / 44));
            int day = (int)(l + 28 - 31 * (month / 4));

            return new DateTime(year, month, day);

        }
        private static DateTime getGaussianEaster(int year)
        {

            int k = year / 100;
            int tmp = (3 * k + 3) / 4;
            int m = 15 + tmp - (8 * k + 13) / 25;
            int s = 2 - tmp;
            int a = year % 19;
            int d = (19 * a + m) % 30;
            int r = (d / 29) + (d / 28 - d / 29) * (a / 11);
            int og = 21 + d - r;
            int sz = 7 - (year + year / 4 + s) % 7;
            int oe = 7 - (og - sz) % 7;
            int os = og + oe - 1;

            DateTime cal = new DateTime(year, 3, 1);

            cal.AddDays(os);

            return (cal);
        }
    }
}
