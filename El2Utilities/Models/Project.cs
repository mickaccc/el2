﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class Project
{
    public string ProjectPsp { get; set; } = null!;

    public int ProjectType { get; set; }

    public string? ProjectInfo { get; set; }

    public string? ProjectColor { get; set; }

    public bool ProjectPrio { get; set; }

    public virtual ICollection<OrderRb> OrderRbs { get; set; } = new List<OrderRb>();

    public virtual ICollection<ProjectAttachment> ProjectAttachments { get; set; } = new List<ProjectAttachment>();
}