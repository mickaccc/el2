﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Utilities.Models;

public partial class UserCost
{
    public string UsrIdent { get; set; } = null!;

    public int CostId { get; set; }

    public virtual Costunit Cost { get; set; } = null!;

    public virtual User UsrIdentNavigation { get; set; } = null!;
}