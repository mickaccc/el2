using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblProjekt
    {
        public TblProjekt()
        {
            TblAuftrags = new HashSet<TblAuftrag>();
            TblProjektAnhangs = new HashSet<TblProjektAnhang>();
        }

        public int Pid { get; set; }
        public string Projekt { get; set; }
        public string Projektinfo { get; set; }
        public string ProjektFarbe { get; set; }
        public string ProjektArt { get; set; }

        public virtual ICollection<TblAuftrag> TblAuftrags { get; set; }
        public virtual ICollection<TblProjektAnhang> TblProjektAnhangs { get; set; }
    }
}
