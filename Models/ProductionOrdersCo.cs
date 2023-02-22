using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class ProductionOrdersCo
{
    public string Idnr { get; set; } = null!;

    public string OrderNumber { get; set; } = null!;

    public string? MaterialNumber { get; set; }

    public string? MaterialDesc { get; set; }

    public string? ProductionOrderHeaderStatus { get; set; }

    public DateTime? ScheduledStartDate { get; set; }

    public DateTime? ActualStartDate { get; set; }

    public DateTime? ScheduledFinish { get; set; }

    public DateTime? ActualFinishDate { get; set; }

    public short OperationNumber { get; set; }

    public int? WorkCenter { get; set; }

    public string? OperationShortText { get; set; }

    public DateTime? ScheduledExecutionStartDate { get; set; }

    public string? ScheduledExecutionStartTime { get; set; }

    public DateTime? ScheduledExecutionFinishDate { get; set; }

    public string? ScheduledExecutionFinishTime { get; set; }

    public int? ProcessTime { get; set; }

    public string? ProcessingTimeUom { get; set; }

    public int? BaseQuantity { get; set; }

    public int? OperationQuantity { get; set; }

    public int? TotalYieldConfirmed { get; set; }

    public int? TotalReworkQuantity { get; set; }

    public int? TotalScrapQuantityConfirmed { get; set; }

    public DateTime? ActualExecutionStartDate { get; set; }

    public string? ActualExecutionStartTime { get; set; }

    public DateTime? ActualExecutionFinishDate { get; set; }

    public string? ActualExecutionFinishTime { get; set; }

    public string? ProductionOrderStatusForProcess { get; set; }

    public DateTime? ZeitStempelAenderung { get; set; }

    public string? ControlKey { get; set; }
}
