using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("WorkSap")]
    public partial class WorkSap
    {
        public WorkSap()
        {
            Vorgangs = new HashSet<Vorgang>();
        }

        [Key]
        [StringLength(255)]
        [Unicode(false)]
        public string WorkSapId { get; set; } = null!;
        public int? RessourceId { get; set; }
        [Column("created", TypeName = "date")]
        public DateTime? Created { get; set; }

        [InverseProperty(nameof(Vorgang.ArbPlSapNavigation))]
        public virtual ICollection<Vorgang> Vorgangs { get; set; }
    }
}
