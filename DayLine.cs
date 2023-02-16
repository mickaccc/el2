using System;
using System.Collections.Generic;
using System.Linq;
using Lieferliste_WPF.Entities;
using Lieferliste_WPF.Working;

namespace Lieferliste_WPF
{
    [Serializable]
    public class DayLine : ObservableList<Stripe>
    {
        private DateTime mDay;
        public DateTime Updated { get; set; }

        #region Constructors

        public DayLine(DateTime tmpDt)
        {
            this.mDay = tmpDt;
        }
        #endregion Constructors

        public DateTime Day
        {
            get { return mDay; }
            set
            {
                mDay = value;
                
            }
        }


        public List<int> getWorkingMinutes()
        {
            List<int> wreg = new List<int>();
            List<int> preg = new List<int>();

            try
            {
                foreach (Stripe ws in this)
                {
                    if (ws != null)
                    {
                        switch (ws.Type)
                        {
                            case 0:
                                for (int i = ws.Start; i <= ws.End; i++)
                                {
                                    if (!preg.Contains(i))
                                    {
                                        preg.Add(i);
                                    }
                                }
                                break;

                            default:
                                if (ws.Start < ws.End)
                                {
                                    for (int i = ws.Start; i <= ws.End; i++)
                                    {
                                        if (!wreg.Contains(i))
                                        {
                                            wreg.Add(i);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }

                wreg.Sort();

                return wreg.Except(preg).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
        public int getCapacity()
        {
            return getWorkingMinutes().Count();
        }


        protected class StripeComparer : IComparer<Stripe>
        {
            public int Compare(Stripe x, Stripe y)
            {
                if (!(y == null))
                {
                    return Math.Sign(x.Start.CompareTo(y.Start));
                }
                else { return 0; }
            }
        }

    }

}

