﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class ProjectAttachment
{
    public int AttachId { get; set; }

    public string? Attachment { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}