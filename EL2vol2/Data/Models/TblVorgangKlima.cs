using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models;

public partial class TblVorgangKlima
{
    public int VorgKlimaId { get; set; }

    public string? Vid { get; set; }

    public int? AnzahlDruck { get; set; }

    public DateTime? Timestamp { get; set; }
}
