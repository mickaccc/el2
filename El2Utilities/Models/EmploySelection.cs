﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class EmploySelection
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public bool Active { get; set; }

    public virtual ICollection<EmployeeNote> EmployeeNotes { get; set; } = new List<EmployeeNote>();
}