﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Utilities.Models;

public partial class TblMaterial
{
    public string Ttnr { get; set; } = null!;

    public string? Bezeichng { get; set; }

    public virtual ICollection<TblAuftrag> TblAuftrags { get; set; } = new List<TblAuftrag>();
}