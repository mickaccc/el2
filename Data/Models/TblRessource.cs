using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblRessource
    {
        public TblRessource()
        {
            TblArbeitsplatzZuteilungs = new HashSet<TblArbeitsplatzZuteilung>();
            TblRessourceVorgangs = new HashSet<TblRessourceVorgang>();
        }

        public int Rid { get; set; }
        public string? RessName { get; set; }
        public string? Abteilung { get; set; }
        public string? Info { get; set; }
        public string? Inventarnummer { get; set; }
        public int? Sort { get; set; }
        public int? Type { get; set; }

        public virtual ICollection<TblArbeitsplatzZuteilung> TblArbeitsplatzZuteilungs { get; set; }
        public virtual ICollection<TblRessourceVorgang> TblRessourceVorgangs { get; set; }
    }
}
