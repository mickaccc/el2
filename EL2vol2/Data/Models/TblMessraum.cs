using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblMessraum")]
    public partial class TblMessraum
    {
        [Key]
        [Column("MRID")]
        public int Mrid { get; set; }
        [Column("VID")]
        [StringLength(255)]
        public string? Vid { get; set; }
        [Column("VorgMRID")]
        public int? VorgMrid { get; set; }
        [Column("MT_Start", TypeName = "datetime")]
        public DateTime? MtStart { get; set; }
        [Column("MT_User_Start")]
        [StringLength(10)]
        public string? MtUserStart { get; set; }
        [Column("MT_Ende", TypeName = "datetime")]
        public DateTime? MtEnde { get; set; }
        [Column("MT_User_Ende")]
        [StringLength(10)]
        public string? MtUserEnde { get; set; }
        [Column("MT_INFO")]
        public string? MtInfo { get; set; }
        [Column("AbarbID")]
        public int? AbarbId { get; set; }
        [Column("MSF_Ende", TypeName = "datetime")]
        public DateTime? MsfEnde { get; set; }
        [Column("MSF_User_Ende")]
        [StringLength(255)]
        public string? MsfUserEnde { get; set; }
        [Column("MSF")]
        public bool Msf { get; set; }
        [Column("MSF_INFO")]
        public string? MsfInfo { get; set; }
        [Column("erledigt")]
        public bool Erledigt { get; set; }
    }
}
