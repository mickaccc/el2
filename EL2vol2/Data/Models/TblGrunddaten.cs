using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblGrunddaten")]
    public partial class TblGrunddaten
    {
        [Key]
        [Column("GID")]
        public int Gid { get; set; }
        [StringLength(50)]
        public string? Kuerzel { get; set; }
        [StringLength(255)]
        public string? Wert { get; set; }
        [StringLength(255)]
        public string? Beschreibung { get; set; }
        [StringLength(50)]
        public string? Text { get; set; }
    }
}
