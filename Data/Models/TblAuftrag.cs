﻿using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblAuftrag
    {
        public TblAuftrag()
        {
            TblAbfrageblatts = new HashSet<TblAbfrageblatt>();
            TblVorgangs = new HashSet<TblVorgang>();
        }

        public string Aid { get; set; } = null!;
        public DateTime? Eckstart { get; set; }
        public DateTime? Eckende { get; set; }
        public DateTime? Timestamp { get; set; }
        public string? AuftragFarbe { get; set; }
        public string? Material { get; set; }
        public string? DummyMat { get; set; }
        public int? ProId { get; set; }
        public DateTime? Iststart { get; set; }
        public DateTime? Istende { get; set; }
        public string? LieferTermin { get; set; }
        public bool Abgeschlossen { get; set; }
        public string? Prio { get; set; }
        public bool Fertig { get; set; }
        public bool Dringend { get; set; }
        public string? Bemerkung { get; set; }
        public string? SysStatus { get; set; }
        public string? AuftragArt { get; set; }
        public bool Ausgebl { get; set; }
        public bool Mappe { get; set; }
        public int? Quantity { get; set; }

        public virtual TblDummy? DummyMatNavigation { get; set; }
        public virtual TblMaterial? MaterialNavigation { get; set; }
        public virtual TblProjekt? Pro { get; set; }
        public virtual ICollection<TblAbfrageblatt> TblAbfrageblatts { get; set; }
        public virtual ICollection<TblVorgang> TblVorgangs { get; set; }
    }
}
