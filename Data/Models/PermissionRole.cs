using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    public partial class PermissionRole
    {
        [Column("created", TypeName = "datetime")]
        public DateTime Created { get; set; }
        [Key]
        public int RoleKey { get; set; }
        [Key]
        [StringLength(15)]
        public string PermissionKey { get; set; } = null!;

        [ForeignKey(nameof(PermissionKey))]
        [InverseProperty(nameof(Permission.PermissionRoles))]
        public virtual Permission PermissionKeyNavigation { get; set; } = null!;
        [ForeignKey(nameof(RoleKey))]
        [InverseProperty(nameof(Role.PermissionRoles))]
        public virtual Role RoleKeyNavigation { get; set; } = null!;
    }
}
