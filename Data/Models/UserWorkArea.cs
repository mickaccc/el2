using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("UserWorkArea")]
    public partial class UserWorkArea
    {
        [Key]
        [Column("WorkAreaID")]
        public int WorkAreaId { get; set; }
        [Key]
        [Column("UserID")]
        [StringLength(255)]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        [InverseProperty("UserWorkAreas")]
        public virtual User User { get; set; } = null!;
        [ForeignKey(nameof(WorkAreaId))]
        [InverseProperty("UserWorkAreas")]
        public virtual WorkArea WorkArea { get; set; } = null!;
    }
}
