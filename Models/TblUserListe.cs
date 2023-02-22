using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblUserListe
{
    public string? Name { get; set; }

    public string? Personalnummer { get; set; }

    public string UserIdent { get; set; } = null!;

    public int? Gruppe { get; set; }

    public int? Bereich { get; set; }

    public string? Email { get; set; }

    public string? Info { get; set; }

    public bool InfoAnzeigen { get; set; }

    public bool Exited { get; set; }

    public virtual ICollection<PerspectUser> PerspectUsers { get; } = new List<PerspectUser>();

    public virtual ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
}
