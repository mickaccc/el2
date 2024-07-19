﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace El2Core.Models;

public partial class DB_COS_LIEFERLISTE_SQLContext : DbContext
{
    public DB_COS_LIEFERLISTE_SQLContext(DbContextOptions<DB_COS_LIEFERLISTE_SQLContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountCost> AccountCosts { get; set; }

    public virtual DbSet<AccountVorgang> AccountVorgangs { get; set; }

    public virtual DbSet<AccountWorkArea> AccountWorkAreas { get; set; }

    public virtual DbSet<Costunit> Costunits { get; set; }

    public virtual DbSet<IdmAccount> IdmAccounts { get; set; }

    public virtual DbSet<IdmRelation> IdmRelations { get; set; }

    public virtual DbSet<IdmRole> IdmRoles { get; set; }

    public virtual DbSet<InMemoryMsg> InMemoryMsgs { get; set; }

    public virtual DbSet<InMemoryOnline> InMemoryOnlines { get; set; }

    public virtual DbSet<OrderGroup> OrderGroups { get; set; }

    public virtual DbSet<OrderRb> OrderRbs { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<ProductionOrderFilter> ProductionOrderFilters { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectAttachment> ProjectAttachments { get; set; }

    public virtual DbSet<Ressource> Ressources { get; set; }

    public virtual DbSet<RessourceCostUnit> RessourceCostUnits { get; set; }

    public virtual DbSet<RessourceUser> RessourceUsers { get; set; }

    public virtual DbSet<RessourceWorkshift> RessourceWorkshifts { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<Rule> Rules { get; set; }

    public virtual DbSet<ShiftCover> ShiftCovers { get; set; }

    public virtual DbSet<ShiftPlan> ShiftPlans { get; set; }

    public virtual DbSet<TblDummy> TblDummies { get; set; }

    public virtual DbSet<TblMaterial> TblMaterials { get; set; }

    public virtual DbSet<Vorgang> Vorgangs { get; set; }

    public virtual DbSet<WorkArea> WorkAreas { get; set; }

    public virtual DbSet<WorkSap> WorkSaps { get; set; }

    public virtual DbSet<WorkShift> WorkShifts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountCost>(entity =>
        {
            entity.HasKey(e => new { e.AccountId, e.CostId });

            entity.ToTable("AccountCost");

            entity.Property(e => e.AccountId)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountCosts)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_AccountCost_idm_accounts");

            entity.HasOne(d => d.Cost).WithMany(p => p.AccountCosts)
                .HasForeignKey(d => d.CostId)
                .HasConstraintName("FK_AccountCost_Costunit");
        });

        modelBuilder.Entity<AccountVorgang>(entity =>
        {
            entity.HasKey(e => new { e.AccountId, e.VorgangId });

            entity.ToTable("AccountVorgang");

            entity.Property(e => e.AccountId)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("accountId");
            entity.Property(e => e.VorgangId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("vorgangId");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountVorgangs)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_AccountVorgang_idm_accounts");

            entity.HasOne(d => d.Vorgang).WithMany(p => p.AccountVorgangs)
                .HasForeignKey(d => d.VorgangId)
                .HasConstraintName("FK_AccountVorgang_Vorgang");
        });

        modelBuilder.Entity<AccountWorkArea>(entity =>
        {
            entity.HasKey(e => new { e.AccountId, e.WorkAreaId });

            entity.ToTable("AccountWorkArea");

            entity.Property(e => e.AccountId)
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created");

            entity.HasOne(d => d.Account).WithMany(p => p.AccountWorkAreas)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_AccountWorkArea_idm_accounts");

            entity.HasOne(d => d.WorkArea).WithMany(p => p.AccountWorkAreas)
                .HasForeignKey(d => d.WorkAreaId)
                .HasConstraintName("FK_AccountWorkArea_WorkArea");
        });

        modelBuilder.Entity<Costunit>(entity =>
        {
            entity.ToTable("Costunit");

            entity.Property(e => e.CostunitId)
                .ValueGeneratedNever()
                .HasColumnName("costunitID");
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.PlanRelevance).HasColumnName("plan_relevance");
        });

        modelBuilder.Entity<IdmAccount>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("idm_accounts_pk");

            entity.ToTable("idm_accounts", tb => tb.HasTrigger("idm_accounts_modify_trg"));

            entity.Property(e => e.AccountId)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("department");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastmodified)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("lastmodified");
            entity.Property(e => e.Lastname)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("lastname");
        });

        modelBuilder.Entity<IdmRelation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("idm_relations", tb => tb.HasTrigger("idm_relations_modify_trg"));

            entity.HasIndex(e => new { e.AccountId, e.RoleId }, "idm_relations_uq").IsUnique();

            entity.Property(e => e.AccountId)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("account_id");
            entity.Property(e => e.Lastmodified)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("lastmodified");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Account).WithMany()
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("idm_relations2accounts_fk");

            entity.HasOne(d => d.Role).WithMany()
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("idm_relations2roles_fk");
        });

        modelBuilder.Entity<IdmRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("idm_roles_pk");

            entity.ToTable("idm_roles", tb => tb.HasTrigger("idm_roles_modify_trg"));

            entity.HasIndex(e => e.RoleName, "idm_roles_uq").IsUnique();

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.Lastmodified)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("lastmodified");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<InMemoryMsg>(entity =>
        {
            entity.HasKey(e => e.MsgId)
                .HasName("PK__InMemory__662358934B319009")
                .IsClustered(false);

            entity
                .ToTable("InMemoryMsg")
                .IsMemoryOptimized();

            entity.Property(e => e.MsgId).HasColumnName("MsgID");
            entity.Property(e => e.Invoker).HasMaxLength(255);
            entity.Property(e => e.Operation).HasMaxLength(50);
            entity.Property(e => e.TableName).HasMaxLength(50);

            entity.HasOne(d => d.Onl).WithMany(p => p.InMemoryMsgs)
                .HasForeignKey(d => d.OnlId)
                .HasConstraintName("FK_InMemoryMsg_InMemoryOnline");
        });

        modelBuilder.Entity<InMemoryOnline>(entity =>
        {
            entity.HasKey(e => e.OnlId)
                .HasName("PK__InMemory__A34E616341453DE5")
                .IsClustered(false);

            entity
                .ToTable("InMemoryOnline")
                .IsMemoryOptimized();

            entity.Property(e => e.OnlId).HasColumnName("OnlID");
            entity.Property(e => e.Login)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PcId)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Userid)
                .HasMaxLength(50)
                .IsFixedLength();
        });

        modelBuilder.Entity<OrderGroup>(entity =>
        {
            entity.ToTable("OrderGroup");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Key)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<OrderRb>(entity =>
        {
            entity.HasKey(e => e.Aid).HasName("PK_Order");

            entity.ToTable("OrderRB", tb =>
                {
                    tb.HasTrigger("AuditChangesOrderRB");
                    tb.HasTrigger("AuditInsertOrderRB");
                });

            entity.Property(e => e.Aid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AID");
            entity.Property(e => e.Abgeschlossen).HasColumnName("abgeschlossen");
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
            entity.Property(e => e.MarkCode).HasMaxLength(255);
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
                .HasColumnName("WBSElement");

            entity.HasOne(d => d.DummyMatNavigation).WithMany(p => p.OrderRbs)
                .HasForeignKey(d => d.DummyMat)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Order_tblDummy");

            entity.HasOne(d => d.MaterialNavigation).WithMany(p => p.OrderRbs)
                .HasForeignKey(d => d.Material)
                .HasConstraintName("FK_Order_tblMaterial");

            entity.HasOne(d => d.OrderGroupNavigation).WithMany(p => p.OrderRbs)
                .HasForeignKey(d => d.OrderGroup)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_OrderRB_OrderGroup");

            entity.HasOne(d => d.Pro).WithMany(p => p.OrderRbs)
                .HasForeignKey(d => d.ProId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Order_project");
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

        modelBuilder.Entity<ProductionOrderFilter>(entity =>
        {
            entity.HasKey(e => e.OrderNumber);

            entity.ToTable("ProductionOrderFilter");

            entity.Property(e => e.OrderNumber).HasMaxLength(32);
            entity.Property(e => e.Kommentar).HasMaxLength(50);
            entity.Property(e => e.ZeitStempelÄnderung).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectPsp).HasName("PK_ProjectPSP");

            entity.ToTable("Project");

            entity.Property(e => e.ProjectPsp)
                .HasMaxLength(50)
                .HasColumnName("ProjectPSP");
            entity.Property(e => e.ProjectColor)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<ProjectAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachId);

            entity.ToTable("ProjectAttachment");

            entity.Property(e => e.ProjectPsp).HasMaxLength(50);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.ProjectPspNavigation).WithMany(p => p.ProjectAttachments)
                .HasForeignKey(d => d.ProjectPsp)
                .HasConstraintName("FK_ProjectAttachment_Project");
        });

        modelBuilder.Entity<Ressource>(entity =>
        {
            entity.ToTable("Ressource");

            entity.Property(e => e.RessourceId).HasColumnName("RessourceID");
            entity.Property(e => e.Abteilung).HasMaxLength(15);
            entity.Property(e => e.Info).HasMaxLength(255);
            entity.Property(e => e.Inventarnummer).HasMaxLength(255);
            entity.Property(e => e.ProcessAddable).HasDefaultValue(true);
            entity.Property(e => e.RessName).HasMaxLength(30);
            entity.Property(e => e.Visability).HasDefaultValue(true);

            entity.HasOne(d => d.ShiftPlan).WithMany(p => p.Ressources)
                .HasForeignKey(d => d.ShiftPlanId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Ressource_ShiftPlan");

            entity.HasOne(d => d.WorkArea).WithMany(p => p.Ressources)
                .HasForeignKey(d => d.WorkAreaId)
                .OnDelete(DeleteBehavior.SetNull)
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
        });

        modelBuilder.Entity<RessourceWorkshift>(entity =>
        {
            entity.HasKey(e => new { e.Rid, e.Sid });

            entity.ToTable("RessourceWorkshift");

            entity.Property(e => e.Rid).HasColumnName("rid");
            entity.Property(e => e.Sid).HasColumnName("sid");

            entity.HasOne(d => d.RidNavigation).WithMany(p => p.RessourceWorkshifts)
                .HasForeignKey(d => d.Rid)
                .HasConstraintName("FK_RessourceWorkshift_Ressource");

            entity.HasOne(d => d.SidNavigation).WithMany(p => p.RessourceWorkshifts)
                .HasForeignKey(d => d.Sid)
                .HasConstraintName("FK_RessourceWorkshift_WorkShift");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissKey }).HasName("PK_PermissionsRole");

            entity.ToTable("RolePermission");

            entity.Property(e => e.PermissKey)
                .HasMaxLength(15)
                .IsFixedLength();
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created");

            entity.HasOne(d => d.PermissKeyNavigation).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissKey)
                .HasConstraintName("FK_RolePermission_Permission");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_RolePermission_idm_roles");
        });

        modelBuilder.Entity<Rule>(entity =>
        {
            entity.Property(e => e.RuleData).HasColumnType("xml");
            entity.Property(e => e.RuleName)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.RuleValue)
                .HasMaxLength(50)
                .IsFixedLength();
        });

        modelBuilder.Entity<ShiftCover>(entity =>
        {
            entity.ToTable("ShiftCover");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CoverMask)
                .HasMaxLength(180)
                .IsFixedLength();
            entity.Property(e => e.CoverName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Lock).HasColumnName("lock");
        });

        modelBuilder.Entity<ShiftPlan>(entity =>
        {
            entity.ToTable("ShiftPlan");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fre)
                .HasMaxLength(180)
                .IsFixedLength()
                .HasColumnName("fre");
            entity.Property(e => e.Lock).HasColumnName("lock");
            entity.Property(e => e.Mon)
                .HasMaxLength(180)
                .IsFixedLength()
                .HasColumnName("mon");
            entity.Property(e => e.PlanName).HasMaxLength(50);
            entity.Property(e => e.Sat)
                .HasMaxLength(180)
                .IsFixedLength()
                .HasColumnName("sat");
            entity.Property(e => e.Sun)
                .HasMaxLength(180)
                .IsFixedLength()
                .HasColumnName("sun");
            entity.Property(e => e.Thu)
                .HasMaxLength(180)
                .IsFixedLength()
                .HasColumnName("thu");
            entity.Property(e => e.Tue)
                .HasMaxLength(180)
                .IsFixedLength()
                .HasColumnName("tue");
            entity.Property(e => e.Wed)
                .HasMaxLength(180)
                .IsFixedLength()
                .HasColumnName("wed");
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

        modelBuilder.Entity<Vorgang>(entity =>
        {
            entity.ToTable("Vorgang", tb =>
                {
                    tb.HasTrigger("AuditChangesVorgang");
                    tb.HasTrigger("AuditInsertVorgang");
                });

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
            entity.Property(e => e.Bullet)
                .HasMaxLength(9)
                .HasDefaultValue("#FFFFFFFF");
            entity.Property(e => e.BulletTwo).HasMaxLength(9);
            entity.Property(e => e.CommentMach).IsUnicode(false);
            entity.Property(e => e.MarkCode).HasMaxLength(50);
            entity.Property(e => e.ProcessingUom)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("ProcessingUOM");
            entity.Property(e => e.QuantityMiss).HasColumnName("Quantity-miss");
            entity.Property(e => e.QuantityMissNeo).HasColumnName("Quantity-miss-neo");
            entity.Property(e => e.QuantityRework).HasColumnName("Quantity-rework");
            entity.Property(e => e.QuantityScrap).HasColumnName("Quantity-scrap");
            entity.Property(e => e.QuantityYield).HasColumnName("Quantity-yield");
            entity.Property(e => e.Rid).HasColumnName("RID");
            entity.Property(e => e.Rstze).HasColumnName("RSTZE");
            entity.Property(e => e.RstzeEinheit)
                .HasMaxLength(5)
                .HasColumnName("RSTZE_Einheit");
            entity.Property(e => e.SortPos)
                .HasMaxLength(8)
                .IsFixedLength();
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
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Vorgang_Ressource");
        });

        modelBuilder.Entity<WorkArea>(entity =>
        {
            entity.ToTable("WorkArea");

            entity.Property(e => e.WorkAreaId).HasColumnName("WorkAreaID");
            entity.Property(e => e.Bereich).HasMaxLength(255);
            entity.Property(e => e.Info).HasMaxLength(255);
            entity.Property(e => e.Sort).HasColumnName("SORT");
        });

        modelBuilder.Entity<WorkSap>(entity =>
        {
            entity.ToTable("WorkSap");

            entity.Property(e => e.WorkSapId)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CostId).HasColumnName("CostID");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created");

            entity.HasOne(d => d.Cost).WithMany(p => p.WorkSaps)
                .HasForeignKey(d => d.CostId)
                .HasConstraintName("FK_WorkSap_Costunit");

            entity.HasOne(d => d.Ressource).WithMany(p => p.WorkSaps)
                .HasForeignKey(d => d.RessourceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WorkSap_Ressource");
        });

        modelBuilder.Entity<WorkShift>(entity =>
        {
            entity.HasKey(e => e.Sid);

            entity.ToTable("WorkShift");

            entity.Property(e => e.Sid).HasColumnName("sid");
            entity.Property(e => e.ShiftDef).HasColumnType("xml");
            entity.Property(e => e.ShiftName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}