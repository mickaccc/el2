using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class Permission
{
    public string Berechtigung { get; set; } = null!;

    public string? Description { get; set; }
}
