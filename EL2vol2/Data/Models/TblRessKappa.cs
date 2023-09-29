using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblRessKappa")]
    public partial class TblRessKappa
    {

        /// <summary>
        /// Identity
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        /// <summary>
        /// Resource ID
        /// </summary>
        [Column("RID")]
        public int Rid { get; set; }
        [Column(TypeName = "date")]
        public DateTime Datum { get; set; }

        /// <summary>
        /// Start Schicht 1
        /// </summary>
        [Column("start1")]
        public int? Start1 { get; set; }

        /// <summary>
        /// Ende Schicht 1
        /// </summary>
        [Column("end1")]
        public int? End1 { get; set; }
        [Column("comment1")]
        public string? Comment1 { get; set; }

        /// <summary>
        /// Start Schicht 2
        /// </summary>
        [Column("start2")]
        public int? Start2 { get; set; }

        /// <summary>
        /// Ende Schicht
        /// </summary>
        [Column("end2")]
        public int? End2 { get; set; }
        [Column("comment2")]
        public string? Comment2 { get; set; }

        /// <summary>
        /// Start Schicht 3
        /// </summary>
        [Column("start3")]
        public int? Start3 { get; set; }

        /// <summary>
        /// Ende Schicht 3
        /// </summary>
        [Column("end3")]
        public int? End3 { get; set; }
        [Column("comment3")]
        public string? Comment3 { get; set; }

        /// <summary>
        /// Timestamp to create the Datarow
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("updated", TypeName = "datetime")]
        public DateTime? Updated { get; set; }
    }
}
