using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblArbeitsplatzZuteilung")]
    public partial class TblArbeitsplatzZuteilung
    {
        [Key]
        [Column("ARBZUTID")]
        public int Arbzutid { get; set; }
        [Column("RID")]
        public int? Rid { get; set; }
        [Column("ARBID")]
        [StringLength(255)]
        [Unicode(false)]
        public string? Arbid { get; set; }
        [Column("BID")]
        public int? Bid { get; set; }
        [StringLength(255)]
        public string? ZutName { get; set; }
        public int? Sortiernummer { get; set; }

        [ForeignKey(nameof(Bid))]
        [InverseProperty(nameof(WorkArea.TblArbeitsplatzZuteilungs))]
        public virtual WorkArea? BidNavigation { get; set; }
    }
}
