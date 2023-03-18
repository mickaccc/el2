using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblMm
    {
        public int MmId { get; set; }
        public string Messmaschine { get; set; }
        public int? Maschinentyp { get; set; }
        public string Beschreibung { get; set; }
    }
}
