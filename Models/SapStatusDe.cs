using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class SapStatusDe
{
    public string SysStat { get; set; } = null!;

    public string Stat { get; set; } = null!;

    public string? Status { get; set; }
}
