using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class SapStatusEn
{
    public string SysStat { get; set; } = null!;

    public string? Stat { get; set; }

    public string? Description { get; set; }
}
