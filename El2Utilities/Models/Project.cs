﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class Project
{
    public string Project1 { get; set; } = null!;

    public string? ProjectInfo { get; set; }

    public string? ProjectColor { get; set; }

    public string? ProjectType { get; set; }

    public virtual ICollection<OrderRb> OrderRbs { get; set; } = new List<OrderRb>();
}