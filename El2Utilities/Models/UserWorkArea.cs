﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class UserWorkArea
{
    public int WorkAreaId { get; set; }

    public string UserId { get; set; } = null!;

    public bool FullAccess { get; set; }

    public bool Standard { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual WorkArea WorkArea { get; set; } = null!;
}