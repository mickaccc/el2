﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class OrderDocu
{
    public long DocuId { get; set; }

    public string? OrderId { get; set; }

    public string? VmpbTemplate { get; set; }

    public string? VmpbOriginal { get; set; }

    public DateTime Timestamp { get; set; }

    public bool? InWork { get; set; }

    public virtual OrderRb? Order { get; set; }
}