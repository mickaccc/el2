using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblEinstellTeil
{
    public int EinstId { get; set; }

    public string? Aid { get; set; }

    public string? Ttnr { get; set; }

    public string? DummyMat { get; set; }

    public int? Stück { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? LastModifed { get; set; }

    public bool Verschrottet { get; set; }

    public virtual TblDummy? DummyMatNavigation { get; set; }

    public virtual TblMaterial? TtnrNavigation { get; set; }
}
