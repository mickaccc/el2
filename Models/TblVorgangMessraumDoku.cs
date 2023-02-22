using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblVorgangMessraumDoku
{
    public int Vmdid { get; set; }

    public int? Vid { get; set; }

    public string? Info { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? UserIdent { get; set; }

    public string? Dateiname { get; set; }
}
