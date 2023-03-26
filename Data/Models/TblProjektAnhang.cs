using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblProjektAnhang
    {
        public int Panhid { get; set; }
        public int Pid { get; set; }
        public string? Dateiname { get; set; }
        public string? AnhangInfo { get; set; }
        public string? UserIdent { get; set; }
        public DateTime? Timestamp { get; set; }
        public bool Aktuell { get; set; }

        public virtual TblProjekt PidNavigation { get; set; } = null!;
    }
}
