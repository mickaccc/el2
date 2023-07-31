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
    public partial class TblMazuConfiguration : IEntityTypeConfiguration<TblMazu>
    {
        public void Configure(EntityTypeBuilder<TblMazu> entity)
        {
            entity.Property(e => e.MaZuId).ValueGeneratedNever();

            entity.HasOne(d => d.Ma)
                .WithMany(p => p.TblMazus)
                .HasForeignKey(d => d.MaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblMAZu_tblMA");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<TblMazu> entity);
    }
}
