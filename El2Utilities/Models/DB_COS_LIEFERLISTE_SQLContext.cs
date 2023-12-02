﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace El2Core.Models;

public partial class DB_COS_LIEFERLISTE_SQLContext : DbContext
{
    public DB_COS_LIEFERLISTE_SQLContext()
    {
    }

    public DB_COS_LIEFERLISTE_SQLContext(DbContextOptions<DB_COS_LIEFERLISTE_SQLContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Costunit> Costunits { get; set; }

    public virtual DbSet<Online> Onlines { get; set; }

    public virtual DbSet<OrderRb> OrderRbs { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PermissionRole> PermissionRoles { get; set; }

    public virtual DbSet<Ressource> Ressources { get; set; }

    public virtual DbSet<RessourceCostUnit> RessourceCostUnits { get; set; }

    public virtual DbSet<RessourceUser> RessourceUsers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TblDummy> TblDummies { get; set; }

    public virtual DbSet<TblMaterial> TblMaterials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserCost> UserCosts { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserWorkArea> UserWorkAreas { get; set; }

    public virtual DbSet<Vorgang> Vorgangs { get; set; }

    public virtual DbSet<WorkArea> WorkAreas { get; set; }

    public virtual DbSet<WorkSap> WorkSaps { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Costunit>(entity =>
        {
            entity.ToTable("Costunit");

            entity.Property(e => e.CostunitId)
                .ValueGeneratedNever()
                .HasColumnName("costunitID");
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.PlanRelevance).HasColumnName("plan_relevance");
        });

        modelBuilder.Entity<Online>(entity =>
        {
            entity.HasKey(e => e.Oid);

            entity.ToTable("Online");

            entity.Property(e => e.Oid).HasColumnName("oid");
            entity.Property(e => e.Login)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PcId)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsFixedLength();
        });

        modelBuilder.Entity<OrderRb>(entity =>
        {
            entity.HasKey(e => e.Aid);

            entity.ToTable("OrderRB");

            entity.Property(e => e.Aid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AID");
            entity.Property(e => e.Abgeschlossen).HasColumnName("abgeschlossen");
            entity.Property(e => e.AuftragArt).HasMaxLength(255);
            entity.Property(e => e.AuftragFarbe).HasMaxLength(10);
            entity.Property(e => e.Ausgebl).HasColumnName("ausgebl");
            entity.Property(e => e.Bemerkung).HasMaxLength(255);
            entity.Property(e => e.DummyMat)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Eckende).HasColumnType("datetime");
            entity.Property(e => e.Eckstart).HasColumnType("datetime");
            entity.Property(e => e.Fertig).HasColumnName("fertig");
            entity.Property(e => e.Istende).HasColumnType("datetime");
            entity.Property(e => e.Iststart).HasColumnType("datetime");
            entity.Property(e => e.LieferTermin).HasMaxLength(255);
            entity.Property(e => e.Material).HasMaxLength(255);
            entity.Property(e => e.Mrpcontroller)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MRPController");
            entity.Property(e => e.OrderCategory)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.OrderType)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Prio).HasMaxLength(255);
            entity.Property(e => e.ProId)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("ProID");
            entity.Property(e => e.ProductionSupervisor)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.SysStatus).HasMaxLength(255);
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.Wbselement)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("WBSElement");

            entity.HasOne(d => d.DummyMatNavigation).WithMany(p => p.OrderRbs)
                .HasForeignKey(d => d.DummyMat)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderRB_tblDummy1");

            entity.HasOne(d => d.MaterialNavigation).WithMany(p => p.OrderRbs)
                .HasForeignKey(d => d.Material)
                .HasConstraintName("FK_OrderRB_tblMaterial");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PKey);

            entity.ToTable("Permission");

            entity.Property(e => e.PKey)
                .HasMaxLength(15)
                .IsFixedLength()
                .HasColumnName("pKey");
            entity.Property(e => e.Categorie).HasMaxLength(50);
            entity.Property(e => e.Description).HasColumnType("ntext");
        });

        modelBuilder.Entity<PermissionRole>(entity =>
        {
            entity.HasKey(e => new { e.RoleKey, e.PermissionKey });

            entity.Property(e => e.PermissionKey)
                .HasMaxLength(15)
                .IsFixedLength();
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created");

            entity.HasOne(d => d.PermissionKeyNavigation).WithMany(p => p.PermissionRoles)
                .HasForeignKey(d => d.PermissionKey)
                .HasConstraintName("FK_PermissionRoles_Permission");

            entity.HasOne(d => d.RoleKeyNavigation).WithMany(p => p.PermissionRoles)
                .HasForeignKey(d => d.RoleKey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PermissionRoles_Roles");
        });

        modelBuilder.Entity<Ressource>(entity =>
        {
            entity.ToTable("Ressource");

            entity.Property(e => e.RessourceId).HasColumnName("RessourceID");
            entity.Property(e => e.Abteilung).HasMaxLength(15);
            entity.Property(e => e.Info).HasMaxLength(255);
            entity.Property(e => e.Inventarnummer).HasMaxLength(255);
            entity.Property(e => e.RessName).HasMaxLength(30);
            entity.Property(e => e.Visability).HasDefaultValue(true);

            entity.HasOne(d => d.WorkArea).WithMany(p => p.Ressources)
                .HasForeignKey(d => d.WorkAreaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Ressource_WorkArea");
        });

        modelBuilder.Entity<RessourceCostUnit>(entity =>
        {
            entity.HasKey(e => new { e.Rid, e.CostId });

            entity.ToTable("RessourceCostUnit");

            entity.Property(e => e.Rid).HasColumnName("RID");

            entity.HasOne(d => d.Cost).WithMany(p => p.RessourceCostUnits)
                .HasForeignKey(d => d.CostId)
                .HasConstraintName("FK_RessourceCostUnit_Costunit");

            entity.HasOne(d => d.RidNavigation).WithMany(p => p.RessourceCostUnits)
                .HasForeignKey(d => d.Rid)
                .HasConstraintName("FK_RessourceCostUnit_Ressource");
        });

        modelBuilder.Entity<RessourceUser>(entity =>
        {
            entity.HasKey(e => new { e.UsId, e.Rid });

            entity.ToTable("RessourceUser");

            entity.Property(e => e.UsId)
                .HasMaxLength(255)
                .HasColumnName("UsID");

            entity.HasOne(d => d.RidNavigation).WithMany(p => p.RessourceUsers)
                .HasForeignKey(d => d.Rid)
                .HasConstraintName("FK_RessourceUser_Ressource");

            entity.HasOne(d => d.Us).WithMany(p => p.RessourceUsers)
                .HasForeignKey(d => d.UsId)
                .HasConstraintName("FK_RessourceUser_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasComment("Time of Create")
                .HasColumnType("datetime")
                .HasColumnName("created");
            entity.Property(e => e.Description)
                .HasMaxLength(30)
                .IsFixedLength();
            entity.Property(e => e.Rolelevel).HasColumnName("rolelevel");
        });

        modelBuilder.Entity<TblDummy>(entity =>
        {
            entity.HasKey(e => e.Aid).HasName("PK_tblDummy_1");

            entity.ToTable("tblDummy");

            entity.Property(e => e.Aid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("AID");
            entity.Property(e => e.Mattext).HasColumnName("MATTEXT");
        });

        modelBuilder.Entity<TblMaterial>(entity =>
        {
            entity.HasKey(e => e.Ttnr);

            entity.ToTable("tblMaterial");

            entity.Property(e => e.Ttnr)
                .HasMaxLength(255)
                .HasColumnName("TTNR");
            entity.Property(e => e.Bezeichng).HasMaxLength(256);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserIdent).HasName("PK__User__1C1A74760950743A");

            entity.ToTable("User");

            entity.Property(e => e.UserIdent).HasMaxLength(255);
            entity.Property(e => e.Exited).HasDefaultValue(false);
            entity.Property(e => e.UsrEmail).HasMaxLength(50);
            entity.Property(e => e.UsrGroup).HasMaxLength(50);
            entity.Property(e => e.UsrInfo).HasMaxLength(50);
            entity.Property(e => e.UsrName).IsUnicode(false);
            entity.Property(e => e.UsrRegion).HasMaxLength(50);
        });

        modelBuilder.Entity<UserCost>(entity =>
        {
            entity.HasKey(e => new { e.UsrIdent, e.CostId });

            entity.ToTable("UserCost");

            entity.Property(e => e.UsrIdent)
                .HasMaxLength(255)
                .HasColumnName("usrIdent");
            entity.Property(e => e.CostId).HasColumnName("costId");

            entity.HasOne(d => d.Cost).WithMany(p => p.UserCosts)
                .HasForeignKey(d => d.CostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserCost_Costunit");

            entity.HasOne(d => d.UsrIdentNavigation).WithMany(p => p.UserCosts)
                .HasForeignKey(d => d.UsrIdent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserCost");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserIdent, e.RoleId });

            entity.Property(e => e.UserIdent).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.UserIdentNavigation).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserIdent)
                .HasConstraintName("FK_UserRoles_User");
        });

        modelBuilder.Entity<UserWorkArea>(entity =>
        {
            entity.HasKey(e => new { e.WorkAreaId, e.UserId }).HasName("PK_UserUnion");

            entity.ToTable("UserWorkArea");

            entity.Property(e => e.WorkAreaId).HasColumnName("WorkAreaID");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.UserWorkAreas)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserWorkArea_User");

            entity.HasOne(d => d.WorkArea).WithMany(p => p.UserWorkAreas)
                .HasForeignKey(d => d.WorkAreaId)
                .HasConstraintName("FK_UserWorkArea_WorkArea");
        });

        modelBuilder.Entity<Vorgang>(entity =>
        {
            entity.HasKey(e => e.VorgangId).HasName("PK_tblVorgang");

            entity.ToTable("Vorgang");

            entity.Property(e => e.VorgangId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("VorgangID");
            entity.Property(e => e.ActualEndDate).HasColumnType("datetime");
            entity.Property(e => e.ActualStartDate).HasColumnType("datetime");
            entity.Property(e => e.Aid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AID");
            entity.Property(e => e.Aktuell).HasColumnName("aktuell");
            entity.Property(e => e.ArbPlSap)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ArbPlSAP");
            entity.Property(e => e.BasisMg).HasMaxLength(255);
            entity.Property(e => e.Beaze).HasColumnName("BEAZE");
            entity.Property(e => e.BeazeEinheit)
                .HasMaxLength(5)
                .HasColumnName("BEAZE_Einheit");
            entity.Property(e => e.BemM)
                .IsUnicode(false)
                .HasColumnName("Bem_M");
            entity.Property(e => e.BemMa)
                .IsUnicode(false)
                .HasColumnName("Bem_MA");
            entity.Property(e => e.BemT)
                .IsUnicode(false)
                .HasColumnName("Bem_T");
            entity.Property(e => e.Bullet).HasMaxLength(9);
            entity.Property(e => e.CommentM).HasColumnType("xml");
            entity.Property(e => e.CommentMa).HasColumnType("xml");
            entity.Property(e => e.CommentMach)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CommentT).HasColumnType("xml");
            entity.Property(e => e.Marker)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("marker");
            entity.Property(e => e.ProcessingUom)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("ProcessingUOM");
            entity.Property(e => e.QuantityMiss).HasColumnName("Quantity-miss");
            entity.Property(e => e.QuantityRework).HasColumnName("Quantity-rework");
            entity.Property(e => e.QuantityScrap).HasColumnName("Quantity-scrap");
            entity.Property(e => e.QuantityYield).HasColumnName("Quantity-yield");
            entity.Property(e => e.Rid).HasColumnName("RID");
            entity.Property(e => e.Rstze).HasColumnName("RSTZE");
            entity.Property(e => e.RstzeEinheit)
                .HasMaxLength(5)
                .HasColumnName("RSTZE_Einheit");
            entity.Property(e => e.SpaetEnd).HasColumnType("datetime");
            entity.Property(e => e.SpaetStart).HasColumnType("datetime");
            entity.Property(e => e.Spos).HasColumnName("SPOS");
            entity.Property(e => e.SteuSchl).HasMaxLength(255);
            entity.Property(e => e.SysStatus).HasMaxLength(255);
            entity.Property(e => e.Termin).HasColumnType("datetime");
            entity.Property(e => e.Text).HasMaxLength(150);
            entity.Property(e => e.Visability).HasDefaultValue(true);
            entity.Property(e => e.Vnr).HasColumnName("VNR");
            entity.Property(e => e.Wrtze).HasColumnName("WRTZE");
            entity.Property(e => e.WrtzeEinheit)
                .HasMaxLength(5)
                .HasColumnName("WRTZE_Einheit");

            entity.HasOne(d => d.AidNavigation).WithMany(p => p.Vorgangs)
                .HasForeignKey(d => d.Aid)
                .HasConstraintName("FK_Vorgang_OrderRB");

            entity.HasOne(d => d.ArbPlSapNavigation).WithMany(p => p.Vorgangs)
                .HasForeignKey(d => d.ArbPlSap)
                .HasConstraintName("FK_Vorgang_WorkSap");

            entity.HasOne(d => d.RidNavigation).WithMany(p => p.Vorgangs)
                .HasForeignKey(d => d.Rid)
                .HasConstraintName("FK_Vorgang_Ressource");
        });

        modelBuilder.Entity<WorkArea>(entity =>
        {
            entity.ToTable("WorkArea");

            entity.Property(e => e.WorkAreaId).HasColumnName("WorkAreaID");
            entity.Property(e => e.Abteilung).HasMaxLength(255);
            entity.Property(e => e.Bereich).HasMaxLength(255);
            entity.Property(e => e.Sort).HasColumnName("SORT");
        });

        modelBuilder.Entity<WorkSap>(entity =>
        {
            entity.ToTable("WorkSap");

            entity.Property(e => e.WorkSapId)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("date")
                .HasColumnName("created");

            entity.HasOne(d => d.Cost).WithMany(p => p.WorkSaps)
                .HasForeignKey(d => d.CostId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WorkSap_Costunit");

            entity.HasOne(d => d.Ressource).WithMany(p => p.WorkSaps)
                .HasForeignKey(d => d.RessourceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WorkSap_Ressource");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}