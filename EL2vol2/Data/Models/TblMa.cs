using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblMA")]
    public partial class TblMa
    {
        public TblMa()
        {
            TblMazus = new HashSet<TblMazu>();
        }

        [Key]
        [Column("MaID")]
        public int MaId { get; set; }
        [Column("VID")]
        [StringLength(255)]
        public string? Vid { get; set; }
        public bool Gutteil { get; set; }
        [Column("lfndProzess")]
        public bool LfndProzess { get; set; }
        public bool Lehrenmessung { get; set; }
        [Column("sonstiges")]
        public bool Sonstiges { get; set; }
        public bool Datei { get; set; }
        public bool Papier { get; set; }
        [Column("RID")]
        public int? Rid { get; set; }
        public int? Aussteller { get; set; }
        [StringLength(50)]
        public string? WunschDatum { get; set; }
        [StringLength(50)]
        public string? WunschZeit { get; set; }
        [Column("Bemerkung_MB")]
        public string? BemerkungMb { get; set; }
        [Column("timestamp")]
        [StringLength(50)]
        public string? Timestamp { get; set; }
        [StringLength(50)]
        public string? UserIdent { get; set; }
        [Column("zustand")]
        [StringLength(50)]
        public string? Zustand { get; set; }
        [Column("Bemerkung_Mt")]
        public string? BemerkungMt { get; set; }
        public bool Vorabprogrammierung { get; set; }

        [InverseProperty(nameof(TblMazu.Ma))]
        public virtual ICollection<TblMazu> TblMazus { get; set; }
    }
}
