﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class RolePermission
{
    public long RoleId { get; set; }

    public string PermissKey { get; set; } = null!;

    public DateTime Created { get; set; }

    public virtual Permission PermissKeyNavigation { get; set; } = null!;

    public virtual IdmRole Role { get; set; } = null!;
}