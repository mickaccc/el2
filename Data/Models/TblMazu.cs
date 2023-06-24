using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblMAZu")]
    public partial class TblMazu
    {
        [Key]
        [Column("MaZuID")]
        public int MaZuId { get; set; }
        [Column("MaID")]
        public int? MaId { get; set; }
        [Column("MmID")]
        public int? MmId { get; set; }
        public int? Programmieraufwand { get; set; }
        [Column("RZeit")]
        public int? Rzeit { get; set; }
        [Column("MZeit")]
        public int? Mzeit { get; set; }
        public int? PersNr { get; set; }
        [Column("timestamp", TypeName = "datetime")]
        public DateTime? Timestamp { get; set; }

        [ForeignKey(nameof(MaId))]
        [InverseProperty(nameof(TblMa.TblMazus))]
        public virtual TblMa? Ma { get; set; }
    }
}
