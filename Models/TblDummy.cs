using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblDummy
{
    public string Aid { get; set; } = null!;

    public string? Mattext { get; set; }

    public virtual ICollection<TblAuftrag> TblAuftrags { get; } = new List<TblAuftrag>();

    public virtual ICollection<TblEinstellTeil> TblEinstellTeils { get; } = new List<TblEinstellTeil>();
}
