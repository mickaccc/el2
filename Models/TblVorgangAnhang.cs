using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblVorgangAnhang
{
    public int Vanhid { get; set; }

    public string? Vid { get; set; }

    public string? Dateiname { get; set; }

    public string? AnhangInfo { get; set; }

    public string? Bereich { get; set; }

    public string? UserIdent { get; set; }

    public DateTime? Timestamp { get; set; }

    public bool Aktuell { get; set; }

    public virtual TblVorgang? VidNavigation { get; set; }
}
