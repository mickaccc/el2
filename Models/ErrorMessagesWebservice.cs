using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class ErrorMessagesWebservice
{
    public string? OrderNumber { get; set; }

    public DateTime? Time { get; set; }

    public string? ErrorMsg { get; set; }

    /// <summary>
    /// 100 = Order nicht vorhanden
    /// </summary>
    public int? Returnnr { get; set; }
}
