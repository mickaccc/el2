﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class RessourceUser
{
    public string UsId { get; set; } = null!;

    public int Rid { get; set; }

    public virtual Ressource RidNavigation { get; set; } = null!;

    public virtual User Us { get; set; } = null!;
}