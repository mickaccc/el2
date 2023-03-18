using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class UserRole
    {
        public int Id { get; set; }
        public string UserIdent { get; set; }
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
        public virtual User UserIdentNavigation { get; set; }
    }
}
