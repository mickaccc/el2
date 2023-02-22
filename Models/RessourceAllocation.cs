using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class RessourceAllocation
{
    public string Aid { get; set; } = null!;

    public DateTime? Eckstart { get; set; }

    public DateTime? Eckende { get; set; }

    public short? Vnr { get; set; }

    public string? ArbPlSap { get; set; }

    public string? Text { get; set; }

    public DateTime? SpaetEnd { get; set; }

    public float? Beaze { get; set; }

    public string? BemM { get; set; }

    public string? BemT { get; set; }

    public string? BemMa { get; set; }

    public int? Quantity { get; set; }

    public int? QuantityMiss { get; set; }

    public float? ProcessTime { get; set; }

    public string? RessName { get; set; }

    public string? Abteilung { get; set; }

    public short? Spos { get; set; }

    public string Vid { get; set; } = null!;

    public int? Rid { get; set; }

    public float? Korrect { get; set; }

    public string? Bereich { get; set; }

    public int Bid { get; set; }

    public string? MaterialDescription { get; set; }

    public string? Material { get; set; }

    public int? QuantityYield { get; set; }

    public string? Usr { get; set; }
}
