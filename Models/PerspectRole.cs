using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class PerspectRole
{
    public int Id { get; set; }

    public string? Modified { get; set; }

    public DateTime ModifiedDate { get; set; }

    public int RoleId { get; set; }

    public int PerspectId { get; set; }

    public virtual Perspective Perspect { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
