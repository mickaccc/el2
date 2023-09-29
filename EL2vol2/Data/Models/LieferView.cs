using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lieferliste_WPF.Data.Models
{
    [Keyless]
    public partial class LieferView
    {
        [Column("VorgangID")]
        [StringLength(255)]
        [Unicode(false)]
        public string VorgangId { get; set; } = null!;
        [Column("AID")]
        [StringLength(50)]
        [Unicode(false)]
        public string Aid { get; set; } = null!;
        [Column("VNR")]
        public short Vnr { get; set; }
        [Column("BID")]
        public int? Bid { get; set; }
        [Column("ArbPlSAP")]
        [StringLength(255)]
        [Unicode(false)]
        public string? ArbPlSap { get; set; }
        [StringLength(150)]
        public string? Text { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SpaetStart { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SpaetEnd { get; set; }
        [Column("BEAZE")]
        public float? Beaze { get; set; }
        [Column("RSTZE")]
        public float? Rstze { get; set; }
        [Column("WRTZE")]
        public float? Wrtze { get; set; }
        [Column("BEAZE_Einheit")]
        [StringLength(5)]
        public string? BeazeEinheit { get; set; }
        [Column("RSTZE_Einheit")]
        [StringLength(5)]
        public string? RstzeEinheit { get; set; }
        [Column("WRTZE_Einheit")]
        [StringLength(5)]
        public string? WrtzeEinheit { get; set; }
        [StringLength(255)]
        public string? SysStatus { get; set; }
        [StringLength(255)]
        public string? SteuSchl { get; set; }
        [StringLength(255)]
        public string? BasisMg { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Termin { get; set; }
        [Column("Bem_M")]
        [Unicode(false)]
        public string? BemM { get; set; }
        [Column("Bem_T")]
        [Unicode(false)]
        public string? BemT { get; set; }
        [Column("Bem_MA")]
        [Unicode(false)]
        public string? BemMa { get; set; }
        [Column("aktuell")]
        public bool Aktuell { get; set; }
        [Column("Quantity-scrap")]
        public int? QuantityScrap { get; set; }
        [Column("Quantity-yield")]
        public int? QuantityYield { get; set; }
        [Column("Quantity-miss")]
        public int? QuantityMiss { get; set; }
        [Column("marker")]
        [StringLength(10)]
        public string? Marker { get; set; }
        [Column("ProcessingUOM")]
        [StringLength(16)]
        public string? ProcessingUom { get; set; }
        public float? ProcessTime { get; set; }
        [Column("Quantity-rework")]
        public int? QuantityRework { get; set; }
        [Column("ausgebl")]
        public bool Ausgebl { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActualStartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActualEndDate { get; set; }
        [Column(TypeName = "xml")]
        public string? CommentM { get; set; }
        [Column(TypeName = "xml")]
        public string? CommentT { get; set; }
        [Column(TypeName = "xml")]
        public string? CommentMa { get; set; }
        [StringLength(9)]
        public string? Bullet { get; set; }
        [Column("SPOS")]
        public int? Spos { get; set; }
        [Column("RID")]
        public int? Rid { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string? CommentMach { get; set; }
    }
}
