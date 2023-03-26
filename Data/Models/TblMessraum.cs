using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblMessraum
    {
        public int Mrid { get; set; }
        public string? Vid { get; set; }
        public int? VorgMrid { get; set; }
        public DateTime? MtStart { get; set; }
        public string? MtUserStart { get; set; }
        public DateTime? MtEnde { get; set; }
        public string? MtUserEnde { get; set; }
        public string? MtInfo { get; set; }
        public int? AbarbId { get; set; }
        public DateTime? MsfEnde { get; set; }
        public string? MsfUserEnde { get; set; }
        public bool Msf { get; set; }
        public string? MsfInfo { get; set; }
        public bool Erledigt { get; set; }
    }
}
