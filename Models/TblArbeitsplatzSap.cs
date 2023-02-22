using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblArbeitsplatzSap
{
    public string Arbid { get; set; } = null!;

    public string? Bezeichnung { get; set; }

    public int? Rid { get; set; }

    public virtual ICollection<TblArbeitsplatzZuteilung> TblArbeitsplatzZuteilungs { get; } = new List<TblArbeitsplatzZuteilung>();

    public virtual ICollection<TblVorgang> TblVorgangs { get; } = new List<TblVorgang>();
}
