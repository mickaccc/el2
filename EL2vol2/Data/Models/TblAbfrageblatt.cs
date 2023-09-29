using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblAbfrageblatt")]
    public partial class TblAbfrageblatt
    {
        [Key]
        [Column("ABID")]
        public int Abid { get; set; }
        [Column("ABAID")]
        public int? Abaid { get; set; }
        [Column("AID")]
        [StringLength(50)]
        [Unicode(false)]
        public string? Aid { get; set; }
        public string? Feld3 { get; set; }
        public string? Feld5 { get; set; }
        public string? Feld6 { get; set; }
        public bool Feld7 { get; set; }
        public bool Feld8 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Feld9 { get; set; }
        public bool Feld11 { get; set; }
        public bool Feld12 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Feld13 { get; set; }
        public bool Feld15 { get; set; }
        public bool Feld16 { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Feld17 { get; set; }
        public string? Feld20 { get; set; }
        public int? Variants { get; set; }
        [Column("timestamp", TypeName = "datetime")]
        public DateTime? Timestamp { get; set; }

        [ForeignKey(nameof(Abaid))]
        [InverseProperty(nameof(TblAbfrageblattAuswahl.TblAbfrageblatts))]
        public virtual TblAbfrageblattAuswahl? Aba { get; set; }
        [ForeignKey(nameof(Aid))]
        [InverseProperty(nameof(TblAuftrag.TblAbfrageblatts))]
        public virtual TblAuftrag? AidNavigation { get; set; }
    }
}
