using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("Ressource")]
    public partial class Ressource
    {
        public Ressource()
        {
            RessourceCostUnits = new HashSet<RessourceCostUnit>();
            Vorgangs = new HashSet<Vorgang>();
        }

        [Key]
        [Column("RessourceID")]
        public int RessourceId { get; set; }
        [StringLength(30)]
        public string? RessName { get; set; }
        [StringLength(15)]
        public string? Abteilung { get; set; }
        [StringLength(255)]
        public string? Info { get; set; }
        [StringLength(255)]
        public string? Inventarnummer { get; set; }
        public int? Sort { get; set; }
        public int? Type { get; set; }
        public int? WorkAreaId { get; set; }

        [ForeignKey(nameof(WorkAreaId))]
        [InverseProperty("Ressources")]
        public virtual WorkArea? WorkArea { get; set; }
        [InverseProperty(nameof(RessourceCostUnit.RidNavigation))]
        public virtual ICollection<RessourceCostUnit> RessourceCostUnits { get; set; }
        [InverseProperty(nameof(Vorgang.RidNavigation))]
        public virtual ICollection<Vorgang> Vorgangs { get; set; }
    }
}
