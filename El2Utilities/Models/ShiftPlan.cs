﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class ShiftPlan
{
    public int Id { get; set; }

    public string PlanName { get; set; } = null!;

    public byte[] Sun { get; set; } = null!;

    public byte[] Mon { get; set; } = null!;

    public byte[] Tue { get; set; } = null!;

    public byte[] Wed { get; set; } = null!;

    public byte[] Thu { get; set; } = null!;

    public byte[] Fre { get; set; } = null!;

    public byte[] Sat { get; set; } = null!;

    public bool Lock { get; set; }

    public virtual ICollection<Ressource> Ressources { get; set; } = new List<Ressource>();
}