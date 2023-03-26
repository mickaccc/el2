using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblDummy
    {
        public TblDummy()
        {
            TblAuftrags = new HashSet<TblAuftrag>();
            TblEinstellTeils = new HashSet<TblEinstellTeil>();
        }

        public string Aid { get; set; } = null!;
        public string? Mattext { get; set; }

        public virtual ICollection<TblAuftrag> TblAuftrags { get; set; }
        public virtual ICollection<TblEinstellTeil> TblEinstellTeils { get; set; }
    }
}
