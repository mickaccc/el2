using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblDummy")]
    public partial class TblDummy
    {
        public TblDummy()
        {
            TblAuftrags = new HashSet<TblAuftrag>();
            TblEinstellTeils = new HashSet<TblEinstellTeil>();
        }

        [Key]
        [Column("AID")]
        [StringLength(255)]
        [Unicode(false)]
        public string Aid { get; set; } = null!;
        [Column("MATTEXT")]
        public string? Mattext { get; set; }

        [InverseProperty(nameof(TblAuftrag.DummyMatNavigation))]
        public virtual ICollection<TblAuftrag> TblAuftrags { get; set; }
        [InverseProperty(nameof(TblEinstellTeil.DummyMatNavigation))]
        public virtual ICollection<TblEinstellTeil> TblEinstellTeils { get; set; }
    }
}
