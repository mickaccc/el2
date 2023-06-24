using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblProjektAnhang")]
    public partial class TblProjektAnhang
    {
        [Key]
        [Column("PANHID")]
        public int Panhid { get; set; }
        [Column("PID")]
        public int Pid { get; set; }
        [StringLength(255)]
        public string? Dateiname { get; set; }
        [StringLength(50)]
        public string? AnhangInfo { get; set; }
        [StringLength(50)]
        public string? UserIdent { get; set; }
        [Column("timestamp", TypeName = "datetime")]
        public DateTime? Timestamp { get; set; }
        [Column("aktuell")]
        public bool Aktuell { get; set; }

        [ForeignKey(nameof(Pid))]
        [InverseProperty(nameof(TblProjekt.TblProjektAnhangs))]
        public virtual TblProjekt PidNavigation { get; set; } = null!;
    }
}
