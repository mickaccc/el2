using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblUserListe
    {
        public TblUserListe()
        {
            UserRoles = new HashSet<UserRole>();
        }

        public string Name { get; set; }
        public string Personalnummer { get; set; }
        public string UserIdent { get; set; }
        public int? Gruppe { get; set; }
        public int? Bereich { get; set; }
        public string Email { get; set; }
        public string Info { get; set; }
        public bool InfoAnzeigen { get; set; }
        public bool Exited { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
