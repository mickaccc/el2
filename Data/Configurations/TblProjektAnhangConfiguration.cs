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
    public partial class TblProjektAnhangConfiguration : IEntityTypeConfiguration<TblProjektAnhang>
    {
        public void Configure(EntityTypeBuilder<TblProjektAnhang> entity)
        {
            entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.PidNavigation)
                .WithMany(p => p.TblProjektAnhangs)
                .HasForeignKey(d => d.Pid)
                .HasConstraintName("FK_tblProjektAnhang_tblProjekt");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<TblProjektAnhang> entity);
    }
}