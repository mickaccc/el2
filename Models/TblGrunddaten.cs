using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblGrunddaten
{
    public int Gid { get; set; }

    public string? Kuerzel { get; set; }

    public string? Wert { get; set; }

    public string? Beschreibung { get; set; }

    public string? Text { get; set; }
}
