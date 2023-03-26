using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblMa
    {
        public TblMa()
        {
            TblMazus = new HashSet<TblMazu>();
        }

        public int MaId { get; set; }
        public string? Vid { get; set; }
        public bool Gutteil { get; set; }
        public bool LfndProzess { get; set; }
        public bool Lehrenmessung { get; set; }
        public bool Sonstiges { get; set; }
        public bool Datei { get; set; }
        public bool Papier { get; set; }
        public int? Rid { get; set; }
        public int? Aussteller { get; set; }
        public string? WunschDatum { get; set; }
        public string? WunschZeit { get; set; }
        public string? BemerkungMb { get; set; }
        public string? Timestamp { get; set; }
        public string? UserIdent { get; set; }
        public string? Zustand { get; set; }
        public string? BemerkungMt { get; set; }
        public bool Vorabprogrammierung { get; set; }

        public virtual ICollection<TblMazu> TblMazus { get; set; }
    }
}
