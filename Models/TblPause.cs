using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblPause
{
    public int PauseId { get; set; }

    public int? Anfang { get; set; }

    public int? Ende { get; set; }

    public string? Bemerkung { get; set; }
}
