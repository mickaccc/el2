using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblAbfrageblattAuswahl")]
    public partial class TblAbfrageblattAuswahl
    {
        public TblAbfrageblattAuswahl()
        {
            TblAbfrageblatts = new HashSet<TblAbfrageblatt>();
        }

        [Key]
        [Column("ABAID")]
        public int Abaid { get; set; }
        public int? Position { get; set; }
        [StringLength(255)]
        public string? Felder { get; set; }
        [Column("Felder_Blau")]
        [StringLength(255)]
        public string? FelderBlau { get; set; }
        [Column("aktuell")]
        public bool Aktuell { get; set; }
        [Column("timestamp", TypeName = "datetime")]
        public DateTime? Timestamp { get; set; }
        [StringLength(255)]
        public string? Feld1 { get; set; }
        [StringLength(255)]
        public string? Feld2 { get; set; }
        [StringLength(255)]
        public string? Feld4 { get; set; }
        [StringLength(255)]
        public string? Feld10 { get; set; }
        [StringLength(255)]
        public string? Feld14 { get; set; }
        [StringLength(255)]
        public string? Feld18 { get; set; }
        [StringLength(255)]
        public string? Feld19 { get; set; }
        [StringLength(255)]
        public string? Feld21 { get; set; }

        [InverseProperty(nameof(TblAbfrageblatt.Aba))]
        public virtual ICollection<TblAbfrageblatt> TblAbfrageblatts { get; set; }
    }
}
