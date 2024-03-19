using El2Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lieferliste_WPF.Utilities
{
    internal static class ProcessStripeService
    {
        public static TimeSpan GetProcessLength(Vorgang vorgang, DateTime start)
        {
            var result = vorgang.Rstze + vorgang.Beaze / vorgang.AidNavigation.Quantity * vorgang.QuantityMiss;

            TimeSpan t = TimeSpan.FromMinutes(Convert.ToDouble(result));
            return GetCalculatedLength(t, start, vorgang.RidNavigation);
        }
        private static TimeSpan GetCalculatedLength(TimeSpan timeSpan, DateTime start, Ressource ressource)
        {
            TimeSpan shiftTime = TimeSpan.FromMinutes(180.0);
            var res = timeSpan - shiftTime;
            return res;
        }
    }
}
