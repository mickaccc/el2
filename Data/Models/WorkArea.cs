using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class WorkArea
    {
        public WorkArea()
        {
            TblArbeitsplatzZuteilungs = new HashSet<TblArbeitsplatzZuteilung>();
        }

        public int Bid { get; set; }
        public string Bereich { get; set; }
        public string Abteilung { get; set; }
        public byte? Sort { get; set; }

        public virtual ICollection<TblArbeitsplatzZuteilung> TblArbeitsplatzZuteilungs { get; set; }
    }
}
