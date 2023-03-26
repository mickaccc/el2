using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class PermissionRole
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string UserKey { get; set; } = null!;
        public int RoleKey { get; set; }
        public string PermissionKey { get; set; } = null!;

        public virtual Permission PermissionKeyNavigation { get; set; } = null!;
        public virtual Role RoleKeyNavigation { get; set; } = null!;
    }
}
