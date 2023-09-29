using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("WorkArea")]
    public partial class WorkArea
    {
        public WorkArea()
        {
            Ressources = new HashSet<Ressource>();
            TblArbeitsplatzZuteilungs = new HashSet<TblArbeitsplatzZuteilung>();
            UserWorkAreas = new HashSet<UserWorkArea>();
        }

        [Key]
        [Column("WorkAreaID")]
        public int WorkAreaId { get; set; }
        [StringLength(255)]
        public string? Bereich { get; set; }
        [StringLength(255)]
        public string? Abteilung { get; set; }
        [Column("SORT")]
        public byte? Sort { get; set; }

        [InverseProperty(nameof(Ressource.WorkArea))]
        public virtual ICollection<Ressource> Ressources { get; set; }
        [InverseProperty(nameof(TblArbeitsplatzZuteilung.BidNavigation))]
        public virtual ICollection<TblArbeitsplatzZuteilung> TblArbeitsplatzZuteilungs { get; set; }
        [InverseProperty(nameof(UserWorkArea.WorkArea))]
        public virtual ICollection<UserWorkArea> UserWorkAreas { get; set; }
    }
}
