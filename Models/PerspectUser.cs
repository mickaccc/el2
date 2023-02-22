using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class PerspectUser
{
    public int PerUsrId { get; set; }

    public int? PerspId { get; set; }

    public string? UsrId { get; set; }

    public virtual Perspective? Persp { get; set; }

    public virtual TblUserListe? Usr { get; set; }
}
