using System;
using System.Globalization;

namespace Lieferliste_WPF.Utilities
{
    public class DateUtils
    {
        /// <summary>
        /// Verwaltet die Daten einer Kalenderwoche
        /// </summary>
        public class CalendarWeek : IComparable
        {
            /// <summary>
            /// Das Jahr
            /// </summary>
            public int Year;
            /// <summary>
            /// Die Kalenderwoche
            /// </summary>
            public int Week;

            /// <summary>
            /// Konstruktor
            /// </summary>
            /// <param name="year">Das Jahr</param>
            /// <param name="week">Die Kalenderwoche</param>
            public CalendarWeek(int year, int week)
            {
                this.Year = year;
                this.Week = week;
            }

            public CalendarWeek(String year, String week)
            {
                this.Year = Convert.ToInt32(year);
                this.Week = Convert.ToInt32(week);
            }

            public override String ToString()
            {
                return Convert.ToString(Week) + "/" + Convert.ToString(Year);
            }

            public int CompareTo(object obj)
            {
                if (obj == null) return 1;
                CalendarWeek other = obj as CalendarWeek;
                if (other == null)
                {
                    throw new ArgumentException("Object is not a CalendarWeek!");
                }
                int cal = this.Year * 100 + this.Week;

                return cal.CompareTo(other.Year * 100 + other.Week);
            }
            public override bool Equals(object obj)
            {
                CalendarWeek objWeek = obj as CalendarWeek;
                if (objWeek != null)
                {
                    return (Year == objWeek.Year) && (Week == objWeek.Week);
                }
                return base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return Year.GetHashCode() | Week.GetHashCode();
            }
        }

        /// <summary>
        /// Berechnet die Kalenderwoche eines internationalen Datums
        /// </summary>
        /// <param name="date">Das Datum</param>
        /// <returns>Gibt ein CalendarWeek-Objekt zurück</returns>
        /// <remarks>
        /// Diese Methode berechnet die Kalenderwoche eines Datums
        /// nach der GetWeekOfYear-Methode eines Calendar-Objekts
        /// und korrigiert den darin enthaltenen Fehler.
        /// </remarks>
        public static CalendarWeek GetCalendarWeek(DateTime date)
        {
            // Aktuelle Kultur ermitteln
            CultureInfo currentCulture = CultureInfo.CurrentCulture;

            // Aktuellen Kalender ermitteln
            Calendar calendar = currentCulture.Calendar;

            // Kalenderwoche über das Calendar-Objekt ermitteln
            int calendarWeek = calendar.GetWeekOfYear(date,
                                                      currentCulture.DateTimeFormat.CalendarWeekRule,
                                                      currentCulture.DateTimeFormat.FirstDayOfWeek);

            // Überprüfen, ob eine Kalenderwoche größer als 52
            // ermittelt wurde und ob die Kalenderwoche des Datums
            // in einer Woche 2 ergibt: In diesem Fall hat
            // GetWeekOfYear die Kalenderwoche nicht nach ISO 8601 
            // berechnet (Montag, der 31.12.2007 wird z. B.
            // fälschlicherweise als KW 53 berechnet). 
            // Die Kalenderwoche wird dann auf 1 gesetzt
            if (calendarWeek > 52)
            {
                date = date.AddDays(7);
                int testCalendarWeek = calendar.GetWeekOfYear(date,
                                                              currentCulture.DateTimeFormat.CalendarWeekRule,
                                                              currentCulture.DateTimeFormat.FirstDayOfWeek);
                if (testCalendarWeek == 2)
                    calendarWeek = 1;
            }

            // Das Jahr der Kalenderwoche ermitteln
            int year = date.Year;
            if (calendarWeek == 1 && date.Month == 12)
                year++;
            if (calendarWeek >= 52 && date.Month == 1)
                year--;

            // Die ermittelte Kalenderwoche zurückgeben
            return new CalendarWeek(year, calendarWeek);
        }

        /// <summary>
        /// Berechnet die Kalenderwoche eines deutschen Datums
        /// </summary>
        /// <param name="date">Das Datum</param>
        /// <returns>Gibt ein CalendarWeek-Objekt zurück</returns>
        /// <remarks>
        /// <para>
        /// Diese Methode gilt nur für die deutsche Kultur.
        /// Sie ist wesentlich schneller als die Methode
        /// <see cref="GetInternationalCalendarWeek"/>.
        /// </para>
        /// <para>
        /// Die Berechnung erfolgt nach dem 
        /// C++-Algorithmus von Ekkehard Hess aus einem Beitrag vom
        /// 29.7.1999 in der Newsgroup 
        /// borland.public.cppbuilder.language
        ///(freigegeben zur allgemeinen Verwendung)
        /// </para>
        /// </remarks>
        public static CalendarWeek GetGermanCalendarWeek(DateTime date)
        {
            double a = Math.Floor((14 - (date.Month)) / 12D);
            double y = date.Year + 4800 - a;
            double m = (date.Month) + (12 * a) - 3;

            double jd = date.Day + Math.Floor(((153 * m) + 2) / 5) +
                (365 * y) + Math.Floor(y / 4) - Math.Floor(y / 100) +
                Math.Floor(y / 400) - 32045;

            double d4 = (jd + 31741 - (jd % 7)) % 146097 % 36524 %
                1461;
            double L = Math.Floor(d4 / 1460);
            double d1 = ((d4 - L) % 365) + L;

            // Kalenderwoche ermitteln
            int calendarWeek = (int)Math.Floor(d1 / 7) + 1;

            // Das Jahr der Kalenderwoche ermitteln
            int year = date.Year;
            if (calendarWeek == 1 && date.Month == 12)
                year++;
            if (calendarWeek >= 52 && date.Month == 1)
                year--;

            // Die ermittelte Kalenderwoche zurückgeben
            return new CalendarWeek(year, calendarWeek);
        }
        /// <summary>
        /// gets the date of Monday in a week
        /// </summary>
        /// <param name="year"></param>
        /// <param name="week"></param>
        /// <returns></returns>
		public static DateTime GetMonday(int year, int week)
        {
            // die 1. KW ist die mit mindestens 4 Tagen im Januar des nächsten Jahres
            DateTime dt = new DateTime(year, 1, 4);

            // Beginn auf Montag setzten
            dt = dt.AddDays(-(int)((dt.DayOfWeek != DayOfWeek.Sunday) ? dt.DayOfWeek - 1 : DayOfWeek.Saturday));

            // Wochen dazu addieren
            return dt.AddDays(--week * 7);
        }


    }
}

