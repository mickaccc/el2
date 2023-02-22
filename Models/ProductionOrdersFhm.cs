using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class ProductionOrdersFhm
{
    public string Idnr { get; set; } = null!;

    public string OrderNumber { get; set; } = null!;

    public string? MaterialNumber { get; set; }

    public string? MaterialDesc { get; set; }

    public short OperationNumber { get; set; }

    public int? WorkCenter { get; set; }

    public string? OperationShortText { get; set; }

    public int? BaseQuantity { get; set; }

    public int? OperationQuantity { get; set; }

    public int? TotalYieldConfirmed { get; set; }

    public int? TotalReworkQuantity { get; set; }

    public int? TotalScrapQuantityConfirmed { get; set; }

    public string? ProductionOrderStatusForProcess { get; set; }

    public string? ProcessesExtension1Sw { get; set; }

    public DateTime? ZeitStempelAenderung { get; set; }
}
