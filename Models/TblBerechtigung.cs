using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblBerechtigung
{
    public string Berechtigung { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<PermissionRole> PermissionRoles { get; } = new List<PermissionRole>();
}
