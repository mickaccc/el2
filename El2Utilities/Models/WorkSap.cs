﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class WorkSap
{
    public string WorkSapId { get; set; } = null!;

    public int? RessourceId { get; set; }

    public DateTime? Created { get; set; }

    public virtual ICollection<Vorgang> Vorgangs { get; set; } = new List<Vorgang>();
}