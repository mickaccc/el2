using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblMm")]
    public partial class TblMm
    {
        [Key]
        [Column("MmID")]
        public int MmId { get; set; }
        [StringLength(50)]
        public string? Messmaschine { get; set; }
        public int? Maschinentyp { get; set; }
        public string? Beschreibung { get; set; }
    }
}
