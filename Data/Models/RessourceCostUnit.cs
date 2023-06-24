using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("RessourceCostUnit")]
    public partial class RessourceCostUnit
    {
        [Key]
        [Column("RID")]
        public int Rid { get; set; }
        [Key]
        public int CostId { get; set; }

        [ForeignKey(nameof(CostId))]
        [InverseProperty(nameof(Costunit.RessourceCostUnits))]
        public virtual Costunit Cost { get; set; } = null!;
        [ForeignKey(nameof(Rid))]
        [InverseProperty(nameof(Ressource.RessourceCostUnits))]
        public virtual Ressource RidNavigation { get; set; } = null!;
    }
}
