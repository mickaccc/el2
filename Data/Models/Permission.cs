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

        public string ValidKey { get; set; }
        public string Description { get; set; }

        public virtual ICollection<PermissionRole> PermissionRoles { get; set; }
    }
}
