﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class ShiftPlanDb
{
    public int Planid { get; set; }

    public string ShiftName { get; set; } = null!;

    public string Su { get; set; } = null!;

    public string Mo { get; set; } = null!;

    public string Tu { get; set; } = null!;

    public string We { get; set; } = null!;

    public string Th { get; set; } = null!;

    public string Fr { get; set; } = null!;

    public string Sa { get; set; } = null!;

    public virtual ICollection<Ressource> Ressources { get; set; } = new List<Ressource>();
}