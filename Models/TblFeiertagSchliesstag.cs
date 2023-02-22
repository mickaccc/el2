using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblFeiertagSchliesstag
{
    public int Fesid { get; set; }

    public DateTime? Datum { get; set; }

    public string? Bemerkung { get; set; }
}
