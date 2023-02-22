using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class ProductionOrderFilter
{
    public string OrderNumber { get; set; } = null!;

    public string? Kommentar { get; set; }

    public DateTime? ZeitStempelÄnderung { get; set; }
}
