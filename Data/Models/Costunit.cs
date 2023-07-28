using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("Costunit")]
    public partial class Costunit
    {
        public Costunit()
        {
            RessourceCostUnits = new HashSet<RessourceCostUnit>();
            UserCosts = new HashSet<UserCost>();
        }

        [Key]
        [Column("costunitID")]
        public int CostunitId { get; set; }
        [StringLength(50)]
        public string? Description { get; set; }
        [Column("plan_relevance")]
        public bool PlanRelevance { get; set; }

        [InverseProperty(nameof(RessourceCostUnit.Cost))]
        public virtual ICollection<RessourceCostUnit> RessourceCostUnits { get; set; }
        [InverseProperty(nameof(UserCost.Cost))]
        public virtual ICollection<UserCost> UserCosts { get; set; }
    }
}
