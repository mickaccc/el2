using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class Perspective
{
    public int PerspectId { get; set; }

    public string? PerspectName { get; set; }

    public byte? PerspectType { get; set; }

    public string? PerspectFileName { get; set; }

    public virtual ICollection<PerspectRole> PerspectRoles { get; } = new List<PerspectRole>();

    public virtual ICollection<PerspectUser> PerspectUsers { get; } = new List<PerspectUser>();
}
