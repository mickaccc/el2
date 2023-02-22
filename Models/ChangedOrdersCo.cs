using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class ChangedOrdersCo
{
    public string OrderNumber { get; set; } = null!;

    public short? StatusTabg { get; set; }

    public DateTime? ZeitStempelAenderung { get; set; }
}
