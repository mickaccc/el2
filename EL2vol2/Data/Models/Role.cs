using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    public partial class Role
    {
        public Role()
        {
            PermissionRoles = new HashSet<PermissionRole>();
            UserRoles = new HashSet<UserRole>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [StringLength(30)]
        public string? Description { get; set; }

        /// <summary>
        /// Time of Create
        /// </summary>
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Column("rolelevel")]
        public int? Rolelevel { get; set; }

        [InverseProperty(nameof(PermissionRole.RoleKeyNavigation))]
        public virtual ICollection<PermissionRole> PermissionRoles { get; set; }
        [InverseProperty(nameof(UserRole.Role))]
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
