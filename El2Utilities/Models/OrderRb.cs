﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using El2Core.ViewModelBase;
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class OrderRb : ModelBase
{
    public string Aid { get; set; } = null!;

    public DateTime? Eckstart { get; set; }

    public DateTime? Eckende { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? AuftragFarbe { get; set; }

    public string? Material { get; set; }

    public string? DummyMat { get; set; }

    public string? ProId { get; set; }

    public DateTime? Iststart { get; set; }

    public DateTime? Istende { get; set; }

    public string? LieferTermin { get; set; }

    public bool Abgeschlossen { get; set; }

    public string? Prio { get; set; }

    public bool Fertig { get; set; }

    public bool Dringend { get; set; }

    public string? Bemerkung { get; set; }

    public string? SysStatus { get; set; }

    public string? AuftragArt { get; set; }

    public bool Ausgebl { get; set; }

    public bool Mappe { get; set; }

    public int? Quantity { get; set; }

    public string? Mrpcontroller { get; set; }

    public string? OrderType { get; set; }

    public string? ProductionSupervisor { get; set; }

    public string? OrderCategory { get; set; }

    public string? Wbselement { get; set; }

    public virtual TblDummy? DummyMatNavigation { get; set; }

    public virtual TblMaterial? MaterialNavigation { get; set; }

    public virtual ICollection<Vorgang> Vorgangs { get; set; } = new List<Vorgang>();
}