﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class InMemoryMsg
{
    public int MsgId { get; set; }

    public string? TableName { get; set; }

    public string? Operation { get; set; }

    public string? PrimaryKey { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public int? OnlId { get; set; }

    public string? Invoker { get; set; }

    public virtual InMemoryOnline? Onl { get; set; }
}