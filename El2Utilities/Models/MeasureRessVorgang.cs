﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class MeasureRessVorgang
{
    public int MessId { get; set; }

    public string VorgId { get; set; } = null!;

    public DateTime Created { get; set; }

    public virtual MeasureRess Mess { get; set; } = null!;

    public virtual Vorgang Vorg { get; set; } = null!;
}