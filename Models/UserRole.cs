using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class UserRole
{
    public int Id { get; set; }

    public string UserIdent { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual TblUserListe UserIdentNavigation { get; set; } = null!;
}
