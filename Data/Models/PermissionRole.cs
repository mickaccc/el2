using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class PermissionRole
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public int RoleKey { get; set; }
        public string PermissionKey { get; set; }

        public virtual Permission PermissionKeyNavigation { get; set; }
        public virtual Role RoleKeyNavigation { get; set; }
    }
}
