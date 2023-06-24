using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("tblMessraumAbarbeitung")]
    public partial class TblMessraumAbarbeitung
    {
        [Key]
        [Column("AbarbID")]
        public int AbarbId { get; set; }
        [StringLength(255)]
        public string? Info { get; set; }
    }
}
