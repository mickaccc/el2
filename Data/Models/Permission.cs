using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class Permission
    {
        public Permission()
        {
            PermissionRoles = new HashSet<PermissionRole>();
        }

        public string PKey { get; set; } = null!;
        public string? Description { get; set; }
        public string? Categorie { get; set; }

        public virtual ICollection<PermissionRole> PermissionRoles { get; set; }
    }
}
