﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class Stopage
{
    public int Id { get; set; }

    public int Rid { get; set; }

    public DateTime Starttime { get; set; }

    public DateTime Endtime { get; set; }

    public string? Description { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual Ressource RidNavigation { get; set; } = null!;
}