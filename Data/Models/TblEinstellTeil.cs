using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblEinstellTeil")]
    public partial class TblEinstellTeil
    {
        [Key]
        [Column("EinstID")]
        public int EinstId { get; set; }
        [Column("AID")]
        [StringLength(50)]
        public string? Aid { get; set; }
        [Column("TTNR")]
        [StringLength(255)]
        public string? Ttnr { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string? DummyMat { get; set; }
        public int? Stück { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime? Created { get; set; }
        [Column("lastModifed", TypeName = "datetime")]
        public DateTime? LastModifed { get; set; }
        [Column("verschrottet")]
        public bool Verschrottet { get; set; }

        [ForeignKey(nameof(DummyMat))]
        [InverseProperty(nameof(TblDummy.TblEinstellTeils))]
        public virtual TblDummy? DummyMatNavigation { get; set; }
        [ForeignKey(nameof(Ttnr))]
        [InverseProperty(nameof(TblMaterial.TblEinstellTeils))]
        public virtual TblMaterial? TtnrNavigation { get; set; }
    }
}
