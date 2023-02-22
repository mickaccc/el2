using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblAbfrageblattAuswahl
{
    public int Abaid { get; set; }

    public int? Position { get; set; }

    public string? Felder { get; set; }

    public string? FelderBlau { get; set; }

    public bool Aktuell { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? Feld1 { get; set; }

    public string? Feld2 { get; set; }

    public string? Feld4 { get; set; }

    public string? Feld10 { get; set; }

    public string? Feld14 { get; set; }

    public string? Feld18 { get; set; }

    public string? Feld19 { get; set; }

    public string? Feld21 { get; set; }

    public virtual ICollection<TblAbfrageblatt> TblAbfrageblatts { get; } = new List<TblAbfrageblatt>();
}
