﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class WorkShift
{
    public int Sid { get; set; }

    public string ShiftName { get; set; } = null!;

    public string ShiftDef { get; set; } = null!;

    public int ShiftType { get; set; }

    public virtual ICollection<RessourceWorkshift> RessourceWorkshifts { get; set; } = new List<RessourceWorkshift>();
}