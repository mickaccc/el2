using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("User")]
    public partial class User
    {
        public User()
        {
            UserCosts = new HashSet<UserCost>();
            UserRoles = new HashSet<UserRole>();
            UserWorkAreas = new HashSet<UserWorkArea>();
        }

        [Key]
        [StringLength(255)]
        public string UserIdent { get; set; } = null!;
        public int? Personalnumber { get; set; }
        [Unicode(false)]
        public string UsrName { get; set; } = null!;
        [StringLength(50)]
        public string? UsrGroup { get; set; }
        [StringLength(50)]
        public string? UsrRegion { get; set; }
        [StringLength(50)]
        public string? UsrEmail { get; set; }
        [StringLength(50)]
        public string? UsrInfo { get; set; }
        public bool? Exited { get; set; }

        [InverseProperty(nameof(UserCost.UsrIdentNavigation))]
        public virtual ICollection<UserCost> UserCosts { get; set; }
        [InverseProperty(nameof(UserRole.UserIdentNavigation))]
        public virtual ICollection<UserRole> UserRoles { get; set; }
        [InverseProperty(nameof(UserWorkArea.User))]
        public virtual ICollection<UserWorkArea> UserWorkAreas { get; set; }
    }
}
