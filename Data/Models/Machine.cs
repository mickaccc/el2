using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("Machine")]
    public partial class Machine
    {
        [Key]
        [Column("maId")]
        public int MaId { get; set; }
        [Column("maName")]
        [StringLength(50)]
        public string? MaName { get; set; }
        [Column("maArt")]
        [StringLength(50)]
        public string? MaArt { get; set; }
        [StringLength(50)]
        public string? InventNo { get; set; }
        public int? CostUnit { get; set; }

        [ForeignKey(nameof(CostUnit))]
        [InverseProperty(nameof(Costunit.Machines))]
        public virtual Costunit? CostUnitNavigation { get; set; }
    }
}
