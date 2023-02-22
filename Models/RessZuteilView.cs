using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class RessZuteilView
{
    public int? Rid { get; set; }

    public int? Bid { get; set; }

    public string? Arbid { get; set; }

    public string? Bezeichnung { get; set; }

    public string? RessName { get; set; }

    public string? Info { get; set; }

    public string? Inventarnummer { get; set; }

    public int? Type { get; set; }

    public int? Arbzutid { get; set; }

    public string? Bereich { get; set; }

    public string? Abteilung { get; set; }

    public byte? Sort { get; set; }

    public string? Material { get; set; }

    public string? MaterialDescription { get; set; }

    public int? Quantity { get; set; }

    public short? Spos { get; set; }

    public float? Korrect { get; set; }

    public string Vid { get; set; } = null!;

    public short? Vnr { get; set; }

    public string? Text { get; set; }

    public float? Beaze { get; set; }

    public DateTime? SpaetEnd { get; set; }

    public string? BemM { get; set; }

    public string? BemT { get; set; }

    public string? BemMa { get; set; }

    public int? QuantityScrap { get; set; }

    public int? QuantityYield { get; set; }

    public int? QuantityMiss { get; set; }

    public float? ProcessTime { get; set; }

    public int? QuantityRework { get; set; }

    public int? Vgrid { get; set; }

    public string Aid { get; set; } = null!;
}
