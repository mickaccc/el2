using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblMaterial
{
    public string Ttnr { get; set; } = null!;

    public string? Bezeichng { get; set; }

    public virtual ICollection<TblAuftrag> TblAuftrags { get; } = new List<TblAuftrag>();

    public virtual ICollection<TblEinstellTeil> TblEinstellTeils { get; } = new List<TblEinstellTeil>();
}
