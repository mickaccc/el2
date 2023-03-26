using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class Role
    {
        public Role()
        {
            PermissionRoles = new HashSet<PermissionRole>();
            UserRoles = new HashSet<UserRole>();
        }

        public int Id { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// Time of Create
        /// </summary>
        public DateTime Created { get; set; }

        public virtual ICollection<PermissionRole> PermissionRoles { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
