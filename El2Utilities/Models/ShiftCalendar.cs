﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;

namespace El2Core.Models;

public partial class ShiftCalendar
{
    public int Id { get; set; }

    public string CalendarName { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public bool Lock { get; set; }

    public bool Repeat { get; set; }

    public virtual ICollection<Ressource> Ressources { get; set; } = new List<Ressource>();

    public virtual ICollection<ShiftCalendarShiftPlan> ShiftCalendarShiftPlans { get; set; } = new List<ShiftCalendarShiftPlan>();
}