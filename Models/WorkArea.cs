using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class WorkArea
{
    public int Bid { get; set; }

    public string? Bereich { get; set; }

    public string? Abteilung { get; set; }

    public byte? Sort { get; set; }

    public virtual ICollection<TblArbeitsplatzZuteilung> TblArbeitsplatzZuteilungs { get; } = new List<TblArbeitsplatzZuteilung>();
}
