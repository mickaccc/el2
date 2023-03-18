using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblEinstellteileTran
    {
        public int TransId { get; set; }
        public int? EinstId { get; set; }
        public string Vid { get; set; }
        public DateTime? Created { get; set; }
        public string TransArt { get; set; }
        public int? Pnr { get; set; }
        public string Bemerkung { get; set; }
        public int? Stk { get; set; }
    }
}
