using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("Permission")]
    public partial class Permission
    {
        public Permission()
        {
            PermissionRoles = new HashSet<PermissionRole>();
        }

        [Key]
        [Column("pKey")]
        [StringLength(15)]
        public string PKey { get; set; } = null!;
        [Column(TypeName = "ntext")]
        public string? Description { get; set; }
        [StringLength(50)]
        public string? Categorie { get; set; }

        [InverseProperty(nameof(PermissionRole.PermissionKeyNavigation))]
        public virtual ICollection<PermissionRole> PermissionRoles { get; set; }
    }
}
