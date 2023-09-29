using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("Online")]
    public partial class Online
    {
        [Key]
        [Column("oid")]
        public int Oid { get; set; }
        [StringLength(50)]
        public string UserId { get; set; } = null!;
        [StringLength(50)]
        public string PcId { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime Login { get; set; }
    }
}
