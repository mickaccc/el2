﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class Role
{
    public int Id { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Time of Create
    /// </summary>
    public DateTime Created { get; set; }

    public int? Rolelevel { get; set; }

    public virtual ICollection<PermissionRole> PermissionRoles { get; set; } = new List<PermissionRole>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}