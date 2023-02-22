using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblRessourceVorgang
{
    public int Vgrid { get; set; }

    public int? Rid { get; set; }

    public string? Vid { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? Usr { get; set; }

    public short? Spos { get; set; }

    public short? Kw { get; set; }

    public float? Korrect { get; set; }

    public DateTime? DateCalculated { get; set; }

    public virtual TblRessource? RidNavigation { get; set; }

    public virtual TblVorgang? VidNavigation { get; set; }
}
