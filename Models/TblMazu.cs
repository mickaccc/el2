using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblMazu
{
    public int MaZuId { get; set; }

    public int? MaId { get; set; }

    public int? MmId { get; set; }

    public int? Programmieraufwand { get; set; }

    public int? Rzeit { get; set; }

    public int? Mzeit { get; set; }

    public int? PersNr { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual TblMa? Ma { get; set; }
}
