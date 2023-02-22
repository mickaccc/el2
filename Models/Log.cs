using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Models;

public partial class Log
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Thread { get; set; } = null!;

    public string Level { get; set; } = null!;

    public string Logger { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Exception { get; set; }

    public string? Source { get; set; }

    public string? HostName { get; set; }
}
