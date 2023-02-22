using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class Auftrag
{
    public string? Aid { get; set; }

    public string? Eckstart { get; set; }

    public string? Eckende { get; set; }

    public string? Timestamp { get; set; }

    public string? AuftragFarbe { get; set; }

    public string? Material { get; set; }

    public string? DummyMat { get; set; }

    public string? ProId { get; set; }

    public string? Iststart { get; set; }

    public string? Istende { get; set; }

    public string? LieferTermin { get; set; }

    public string? Abgeschlossen { get; set; }

    public string? Prio { get; set; }

    public string? Fertig { get; set; }

    public string? Dringend { get; set; }

    public string? Bemerkung { get; set; }

    public string? SysStatus { get; set; }

    public string? AuftragArt { get; set; }

    public string? Ausgebl { get; set; }

    public string? Mappe { get; set; }

    public string? Quantity { get; set; }
}
