using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class ChangedOrder
{
    public string OrderNumber { get; set; } = null!;

    public bool StatusTabg { get; set; }

    public DateTime? ZeitStempelAenderung { get; set; }
}
