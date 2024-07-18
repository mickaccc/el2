﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class IdmAccount
{
    public string AccountId { get; set; } = null!;

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Email { get; set; }

    public string? Department { get; set; }

    public DateTime? Lastmodified { get; set; }

    public virtual ICollection<AccountCost> AccountCosts { get; set; } = new List<AccountCost>();

    public virtual ICollection<AccountVorgang> AccountVorgangs { get; set; } = new List<AccountVorgang>();

    public virtual ICollection<AccountWorkArea> AccountWorkAreas { get; set; } = new List<AccountWorkArea>();

    public virtual ICollection<IdmRelation> IdmRelations { get; set; } = new List<IdmRelation>();
}