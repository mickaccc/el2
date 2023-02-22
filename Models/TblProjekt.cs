using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblProjekt
{
    public int Pid { get; set; }

    public string? Projekt { get; set; }

    public string? Projektinfo { get; set; }

    public string? ProjektFarbe { get; set; }

    public string? ProjektArt { get; set; }

    public virtual ICollection<TblAuftrag> TblAuftrags { get; } = new List<TblAuftrag>();

    public virtual ICollection<TblProjektAnhang> TblProjektAnhangs { get; } = new List<TblProjektAnhang>();
}
