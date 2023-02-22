using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class TblUserBerechtigung
{
    public int Id { get; set; }

    public string UserIdent { get; set; } = null!;

    public string Berid { get; set; } = null!;

    public DateTime? Ablaufdatum { get; set; }

    public DateTime? Timestamp { get; set; }
}
