using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class Role
{
    public int Id { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Time of Create
    /// </summary>
    public DateTime Created { get; set; }

    public virtual ICollection<PermissionRole> PermissionRoles { get; } = new List<PermissionRole>();

    public virtual ICollection<PerspectRole> PerspectRoles { get; } = new List<PerspectRole>();

    public virtual ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
}
