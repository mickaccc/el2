using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblMaterial")]
    public partial class TblMaterial
    {
        public TblMaterial()
        {
            TblAuftrags = new HashSet<TblAuftrag>();
            TblEinstellTeils = new HashSet<TblEinstellTeil>();
        }

        [Key]
        [Column("TTNR")]
        [StringLength(255)]
        public string Ttnr { get; set; } = null!;
        [StringLength(256)]
        public string? Bezeichng { get; set; }

        [InverseProperty(nameof(TblAuftrag.MaterialNavigation))]
        public virtual ICollection<TblAuftrag> TblAuftrags { get; set; }
        [InverseProperty(nameof(TblEinstellTeil.TtnrNavigation))]
        public virtual ICollection<TblEinstellTeil> TblEinstellTeils { get; set; }
    }
}
