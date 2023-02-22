using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblRessource
{
    public int Rid { get; set; }

    public string? RessName { get; set; }

    public string? Abteilung { get; set; }

    public string? Info { get; set; }

    public string? Inventarnummer { get; set; }

    public int? Sort { get; set; }

    public int? Type { get; set; }

    public virtual ICollection<TblArbeitsplatzZuteilung> TblArbeitsplatzZuteilungs { get; } = new List<TblArbeitsplatzZuteilung>();

    public virtual ICollection<TblRessourceVorgang> TblRessourceVorgangs { get; } = new List<TblRessourceVorgang>();
}
