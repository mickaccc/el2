using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    [Table("UserCost")]
    public partial class UserCost
    {
        [Key]
        [Column("usrIdent")]
        [StringLength(255)]
        public string UsrIdent { get; set; } = null!;
        [Key]
        [Column("costId")]
        public int CostId { get; set; }

        [ForeignKey(nameof(CostId))]
        [InverseProperty(nameof(Costunit.UserCosts))]
        public virtual Costunit Cost { get; set; } = null!;
        [ForeignKey(nameof(UsrIdent))]
        [InverseProperty(nameof(User.UserCosts))]
        public virtual User UsrIdentNavigation { get; set; } = null!;
    }
}
