﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using Lieferliste_WPF.Data;
using Lieferliste_WPF.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

#nullable disable

namespace Lieferliste_WPF.Data.Configurations
{
    public partial class RessourceCostUnitConfiguration : IEntityTypeConfiguration<RessourceCostUnit>
    {
        public void Configure(EntityTypeBuilder<RessourceCostUnit> entity)
        {
            entity.HasKey(e => new { e.Rid, e.CostId });

            entity.HasOne(d => d.Cost)
                .WithMany(p => p.RessourceCostUnits)
                .HasForeignKey(d => d.CostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RessourceCostUnit_Costunit");

            entity.HasOne(d => d.RidNavigation)
                .WithMany(p => p.RessourceCostUnits)
                .HasForeignKey(d => d.Rid)
                .HasConstraintName("FK_RessourceCostUnit_Ressource");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<RessourceCostUnit> entity);
    }
}
