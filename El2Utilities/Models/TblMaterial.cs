﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class TblMaterial
{
    public string Ttnr { get; set; } = null!;

    public string? Bezeichng { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<OrderRb> OrderRbs { get; set; } = new List<OrderRb>();

    public virtual ICollection<VorgangComponent> VorgangComponents { get; set; } = new List<VorgangComponent>();
}