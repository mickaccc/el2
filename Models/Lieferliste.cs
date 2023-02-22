using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class Lieferliste
{
    public string Aid { get; set; } = null!;

    public string? AuftragFarbe { get; set; }

    public bool Dringend { get; set; }

    public string? Bemerkung { get; set; }

    public string? Prio { get; set; }

    public string? LieferTermin { get; set; }

    public string? Text { get; set; }

    public string? SysStatus { get; set; }

    public short? Vnr { get; set; }

    public DateTime? SpaetEnd { get; set; }

    public string? Vid { get; set; }

    public bool Mappe { get; set; }

    public string? Projekt { get; set; }

    public string? Projektinfo { get; set; }

    public string? ProjektFarbe { get; set; }

    public string? ArbBereich { get; set; }

    public int? ArbBid { get; set; }

    public string? ArbPlSap { get; set; }

    public int? Pid { get; set; }

    public DateTime? SpaetStart { get; set; }

    public bool Fertig { get; set; }

    public string? Arbeitsplatz { get; set; }

    public int? Rid { get; set; }

    public string? RessName { get; set; }

    public string? AuftragArt { get; set; }

    public bool Abgeschlossen { get; set; }

    public string? ProjektArt { get; set; }

    public string? Material { get; set; }

    public string? Teil { get; set; }

    public string? Plantermin { get; set; }

    public int? Quantity { get; set; }

    public int? QuantityYield { get; set; }

    public string? Marker { get; set; }

    public string? BemM { get; set; }

    public string? BemT { get; set; }

    public string? BemMa { get; set; }

    public DateTime? Termin { get; set; }

    public int? QuantityMiss { get; set; }

    public int? QuantityScrap { get; set; }

    public int? QuantityRework { get; set; }

    public bool? Ausgebl { get; set; }

    public float? ProcessTime { get; set; }
}
