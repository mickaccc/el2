using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models
{
    public partial class UserWorkArea
    {
        public int Id { get; set; }
        public int? BerId { get; set; }
        public string UserId { get; set; }
    }
}
