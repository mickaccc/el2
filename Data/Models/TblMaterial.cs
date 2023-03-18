using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblMaterial
    {
        public TblMaterial()
        {
            TblAuftrags = new HashSet<TblAuftrag>();
            TblEinstellTeils = new HashSet<TblEinstellTeil>();
        }

        public string Ttnr { get; set; }
        public string Bezeichng { get; set; }

        public virtual ICollection<TblAuftrag> TblAuftrags { get; set; }
        public virtual ICollection<TblEinstellTeil> TblEinstellTeils { get; set; }
    }
}
