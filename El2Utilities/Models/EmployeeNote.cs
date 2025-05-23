﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using El2Core.ViewModelBase;
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class EmployeeNote : ModelBase
{
    public int Id { get; set; }

    public string AccId { get; set; } = null!;

    public int? SelId { get; set; }

    public string? VorgId { get; set; }

    public string Reference { get; set; } = null!;

    public string? Comment { get; set; }

    public DateTime Date { get; set; }

    public DateTime Timestamp { get; set; }

    public double? Processingtime { get; set; }

    public string? Stk { get; set; }

    public string? Usr { get; set; }

    public virtual IdmAccount Acc { get; set; } = null!;

    public virtual EmploySelection? Sel { get; set; }

    public virtual Vorgang? Vorg { get; set; }
}