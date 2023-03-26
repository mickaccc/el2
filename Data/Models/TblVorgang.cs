using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class TblVorgang
    {
        public TblVorgang()
        {
            TblRessourceVorgangs = new HashSet<TblRessourceVorgang>();
        }

        public string Vid { get; set; } = null!;
        public string? Aid { get; set; }
        public short? Vnr { get; set; }
        public int? Bid { get; set; }
        public string? ArbPlSap { get; set; }
        public string? Text { get; set; }
        public DateTime? SpaetStart { get; set; }
        public DateTime? SpaetEnd { get; set; }
        public float? Beaze { get; set; }
        public float? Rstze { get; set; }
        public float? Wrtze { get; set; }
        public string? BeazeEinheit { get; set; }
        public string? RstzeEinheit { get; set; }
        public string? WrtzeEinheit { get; set; }
        public string? SysStatus { get; set; }
        public string? SteuSchl { get; set; }
        public string? BasisMg { get; set; }
        public DateTime? Termin { get; set; }
        public string? BemM { get; set; }
        public string? BemT { get; set; }
        public string? BemMa { get; set; }
        public bool Aktuell { get; set; }
        public int? QuantityScrap { get; set; }
        public int? QuantityYield { get; set; }
        public int? QuantityMiss { get; set; }
        public string? Marker { get; set; }
        public string? ProcessingUom { get; set; }
        public float? ProcessTime { get; set; }
        public int? QuantityRework { get; set; }
        public bool Ausgebl { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }

        public virtual TblAuftrag? AidNavigation { get; set; }
        public virtual TblArbeitsplatzSap? ArbPlSapNavigation { get; set; }
        public virtual ICollection<TblRessourceVorgang> TblRessourceVorgangs { get; set; }
    }
}
