﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace Lieferliste_WPF.Data.Models;

public partial class Costunit
{
    public int CostunitId { get; set; }

    public string? Description { get; set; }

    public bool PlanRelevance { get; set; }

    public virtual ICollection<RessourceCostUnit> RessourceCostUnits { get; set; } = new List<RessourceCostUnit>();

    public virtual ICollection<UserCost> UserCosts { get; set; } = new List<UserCost>();

    public virtual ICollection<WorkSap> WorkSaps { get; set; } = new List<WorkSap>();
}