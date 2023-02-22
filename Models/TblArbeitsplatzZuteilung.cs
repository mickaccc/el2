using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblArbeitsplatzZuteilung
{
    public int Arbzutid { get; set; }

    public int? Rid { get; set; }

    public string? Arbid { get; set; }

    public int? Bid { get; set; }

    public string? ZutName { get; set; }

    public int? Sortiernummer { get; set; }

    public virtual TblArbeitsplatzSap? Arb { get; set; }

    public virtual WorkArea? BidNavigation { get; set; }

    public virtual TblRessource? RidNavigation { get; set; }
}
