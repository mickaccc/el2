using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblEinstellteileTrans")]
    public partial class TblEinstellteileTran
    {
        [Key]
        [Column("TransID")]
        public int TransId { get; set; }
        [Column("EinstID")]
        public int? EinstId { get; set; }
        [Column("VID")]
        [StringLength(255)]
        public string? Vid { get; set; }
        [Column("created", TypeName = "datetime")]
        public DateTime? Created { get; set; }
        [StringLength(10)]
        public string? TransArt { get; set; }
        [Column("PNR")]
        public int? Pnr { get; set; }
        public string? Bemerkung { get; set; }
        [Column("stk")]
        public int? Stk { get; set; }
    }
}
