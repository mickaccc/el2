﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Statics.Models;

public partial class Permission
{
    public string PKey { get; set; }

    public string Description { get; set; }

    public string Categorie { get; set; }

    public virtual ICollection<PermissionRoles> PermissionRoles { get; set; } = new List<PermissionRoles>();
}