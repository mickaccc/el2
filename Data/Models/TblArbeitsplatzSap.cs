using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblArbeitsplatzSap
    {
        public TblArbeitsplatzSap()
        {
            TblArbeitsplatzZuteilungs = new HashSet<TblArbeitsplatzZuteilung>();
            TblVorgangs = new HashSet<TblVorgang>();
        }

        public string Arbid { get; set; } = null!;
        public string? Bezeichnung { get; set; }
        public int? Rid { get; set; }

        public virtual ICollection<TblArbeitsplatzZuteilung> TblArbeitsplatzZuteilungs { get; set; }
        public virtual ICollection<TblVorgang> TblVorgangs { get; set; }
    }
}
