﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class VorgangAttachment
{
    public int AttachId { get; set; }

    public string VorgangId { get; set; } = null!;

    public byte[]? Data { get; set; }

    public string Link { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public bool IsLink { get; set; }

    public virtual Vorgang Vorgang { get; set; } = null!;
}