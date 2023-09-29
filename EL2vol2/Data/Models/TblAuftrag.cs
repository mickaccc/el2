using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblAuftrag")]
    public partial class TblAuftrag
    {
        public TblAuftrag()
        {
            TblAbfrageblatts = new HashSet<TblAbfrageblatt>();
            Vorgangs = new HashSet<Vorgang>();
        }

        [Key]
        [Column("AID")]
        [StringLength(50)]
        [Unicode(false)]
        public string Aid { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime? Eckstart { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Eckende { get; set; }
        [Column("timestamp", TypeName = "datetime")]
        public DateTime? Timestamp { get; set; }
        [StringLength(10)]
        public string? AuftragFarbe { get; set; }
        [StringLength(255)]
        public string? Material { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string? DummyMat { get; set; }
        [Column("ProID")]
        public int? ProId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Iststart { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Istende { get; set; }
        [StringLength(255)]
        public string? LieferTermin { get; set; }
        [Column("abgeschlossen")]
        public bool Abgeschlossen { get; set; }
        [StringLength(255)]
        public string? Prio { get; set; }
        [Column("fertig")]
        public bool Fertig { get; set; }
        public bool Dringend { get; set; }
        [StringLength(255)]
        public string? Bemerkung { get; set; }
        [StringLength(255)]
        public string? SysStatus { get; set; }
        [StringLength(255)]
        public string? AuftragArt { get; set; }
        [Column("ausgebl")]
        public bool Ausgebl { get; set; }
        public bool Mappe { get; set; }
        public int? Quantity { get; set; }

        [ForeignKey(nameof(DummyMat))]
        [InverseProperty(nameof(TblDummy.TblAuftrags))]
        public virtual TblDummy? DummyMatNavigation { get; set; }
        [ForeignKey(nameof(Material))]
        [InverseProperty(nameof(TblMaterial.TblAuftrags))]
        public virtual TblMaterial? MaterialNavigation { get; set; }
        [ForeignKey(nameof(ProId))]
        [InverseProperty(nameof(TblProjekt.TblAuftrags))]
        public virtual TblProjekt? Pro { get; set; }
        [InverseProperty(nameof(TblAbfrageblatt.AidNavigation))]
        public virtual ICollection<TblAbfrageblatt> TblAbfrageblatts { get; set; }
        [InverseProperty(nameof(Vorgang.AidNavigation))]
        public virtual ICollection<Vorgang> Vorgangs { get; set; }
    }
}
