﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class PermissionRole
{
    public DateTime Created { get; set; }

    public int RoleKey { get; set; }

    public string PermissionKey { get; set; } = null!;

    public virtual Permission PermissionKeyNavigation { get; set; } = null!;

    public virtual Role RoleKeyNavigation { get; set; } = null!;
}