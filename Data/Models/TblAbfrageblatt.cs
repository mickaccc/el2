using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblAbfrageblatt
    {
        public int Abid { get; set; }
        public int? Abaid { get; set; }
        public string? Aid { get; set; }
        public string? Feld3 { get; set; }
        public string? Feld5 { get; set; }
        public string? Feld6 { get; set; }
        public bool Feld7 { get; set; }
        public bool Feld8 { get; set; }
        public DateTime? Feld9 { get; set; }
        public bool Feld11 { get; set; }
        public bool Feld12 { get; set; }
        public DateTime? Feld13 { get; set; }
        public bool Feld15 { get; set; }
        public bool Feld16 { get; set; }
        public DateTime? Feld17 { get; set; }
        public string? Feld20 { get; set; }
        public int? Variants { get; set; }
        public DateTime? Timestamp { get; set; }

        public virtual TblAbfrageblattAuswahl? Aba { get; set; }
        public virtual TblAuftrag? AidNavigation { get; set; }
    }
}
