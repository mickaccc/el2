using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class Vorgang
{
    public string? Vid { get; set; }

    public string? Aid { get; set; }

    public string? Vnr { get; set; }

    public string? Bid { get; set; }

    public string? ArbPlSap { get; set; }

    public string? Text { get; set; }

    public string? SpaetStart { get; set; }

    public string? SpaetEnd { get; set; }

    public string? Beaze { get; set; }

    public string? Rstze { get; set; }

    public string? Wrtze { get; set; }

    public string? BeazeEinheit { get; set; }

    public string? RstzeEinheit { get; set; }

    public string? WrtzeEinheit { get; set; }

    public string? SysStatus { get; set; }

    public string? SteuSchl { get; set; }

    public string? BasisMg { get; set; }

    public string? Termin { get; set; }

    public string? BemM { get; set; }

    public string? BemT { get; set; }

    public string? BemMa { get; set; }

    public string? Aktuell { get; set; }

    public string? QuantityScrap { get; set; }

    public string? QuantityYield { get; set; }

    public string? QuantityMiss { get; set; }

    public string? Marker { get; set; }

    public string? ProcessingUom { get; set; }

    public string? ProcessTime { get; set; }

    public string? QuantityRework { get; set; }

    public string? Ausgebl { get; set; }

    public string? ActualStartDate { get; set; }

    public string? ActualEndDate { get; set; }
}
