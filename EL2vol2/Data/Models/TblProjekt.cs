using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblProjekt")]
    public partial class TblProjekt
    {
        public TblProjekt()
        {
            TblAuftrags = new HashSet<TblAuftrag>();
            TblProjektAnhangs = new HashSet<TblProjektAnhang>();
        }

        [Key]
        [Column("PID")]
        public int Pid { get; set; }
        [StringLength(50)]
        public string? Projekt { get; set; }
        public string? Projektinfo { get; set; }
        [StringLength(10)]
        public string? ProjektFarbe { get; set; }
        [StringLength(255)]
        public string? ProjektArt { get; set; }

        [InverseProperty(nameof(TblAuftrag.Pro))]
        public virtual ICollection<TblAuftrag> TblAuftrags { get; set; }
        [InverseProperty(nameof(TblProjektAnhang.PidNavigation))]
        public virtual ICollection<TblProjektAnhang> TblProjektAnhangs { get; set; }
    }
}
