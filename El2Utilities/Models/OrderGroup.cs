﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class OrderGroup
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public string Key { get; set; } = null!;

    public virtual ICollection<OrderRb> OrderRbs { get; set; } = new List<OrderRb>();
}