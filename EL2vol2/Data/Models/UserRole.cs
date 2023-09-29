using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Data.Models
{
    public partial class UserRole
    {
        [Key]
        [StringLength(255)]
        public string UserIdent { get; set; } = null!;
        [Key]
        [Column("RoleID")]
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        [InverseProperty("UserRoles")]
        public virtual Role Role { get; set; } = null!;
        [ForeignKey(nameof(UserIdent))]
        [InverseProperty(nameof(User.UserRoles))]
        public virtual User UserIdentNavigation { get; set; } = null!;
    }
}
