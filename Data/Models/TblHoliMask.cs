using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblHoliMask")]
    public partial class TblHoliMask
    {
        [Key]
        [Column("_hid")]
        public int Hid { get; set; }
        [Column("holiName")]
        [StringLength(255)]
        public string? HoliName { get; set; }
        [Column("holiDate", TypeName = "datetime")]
        public DateTime? HoliDate { get; set; }
        [Column("holiDescript")]
        [StringLength(255)]
        public string? HoliDescript { get; set; }
        [Column("holiFormula")]
        [StringLength(255)]
        public string? HoliFormula { get; set; }
    }
}
