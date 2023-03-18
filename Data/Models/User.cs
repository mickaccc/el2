using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class User
    {
        public User()
        {
            UserRoles = new HashSet<UserRole>();
        }

        public string UsrName { get; set; }
        public string Personalnumber { get; set; }
        public string UserIdent { get; set; }
        public int? UsrGroup { get; set; }
        public int? UsrRegion { get; set; }
        public string UsrEmail { get; set; }
        public string UsrInfo { get; set; }
        public bool InfoAnzeigen { get; set; }
        public bool Exited { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
