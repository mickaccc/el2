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
    public partial class UserWorkAreaConfiguration : IEntityTypeConfiguration<UserWorkArea>
    {
        public void Configure(EntityTypeBuilder<UserWorkArea> entity)
        {
            entity.HasKey(e => new { e.WorkAreaId, e.UserId })
                .HasName("PK_UserUnion");

            entity.HasOne(d => d.User)
                .WithMany(p => p.UserWorkAreas)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserWorkArea_User");

            entity.HasOne(d => d.WorkArea)
                .WithMany(p => p.UserWorkAreas)
                .HasForeignKey(d => d.WorkAreaId)
                .HasConstraintName("FK_UserWorkArea_WorkArea");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<UserWorkArea> entity);
    }
}
