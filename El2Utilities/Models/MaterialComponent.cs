﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class MaterialComponent
{
    public int Id { get; set; }

    public string Ttnr { get; set; } = null!;

    public string TtnrC { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public virtual Component TtnrCNavigation { get; set; } = null!;

    public virtual TblMaterial TtnrNavigation { get; set; } = null!;
}