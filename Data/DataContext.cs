using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Lieferliste_WPF.Data.Models;

namespace Lieferliste_WPF.Data
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<Costunit> Costunits { get; set; } = null!;
        public virtual DbSet<Machine> Machines { get; set; } = null!;
        public virtual DbSet<Permission> Permissions { get; set; } = null!;
        public virtual DbSet<PermissionRole> PermissionRoles { get; set; } = null!;
        public virtual DbSet<Ressource> Ressources { get; set; } = null!;
        public virtual DbSet<RessourceCostUnit> RessourceCostUnits { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<TblAbfrageblatt> TblAbfrageblatts { get; set; } = null!;
        public virtual DbSet<TblAbfrageblattAuswahl> TblAbfrageblattAuswahls { get; set; } = null!;
        public virtual DbSet<TblArbeitsplatzZuteilung> TblArbeitsplatzZuteilungs { get; set; } = null!;
        public virtual DbSet<TblAuftrag> TblAuftrags { get; set; } = null!;
        public virtual DbSet<TblDummy> TblDummies { get; set; } = null!;
        public virtual DbSet<TblEinstellTeil> TblEinstellTeils { get; set; } = null!;
        public virtual DbSet<TblEinstellteileTran> TblEinstellteileTrans { get; set; } = null!;
        public virtual DbSet<TblGrunddaten> TblGrunddatens { get; set; } = null!;
        public virtual DbSet<TblHoliMask> TblHoliMasks { get; set; } = null!;
        public virtual DbSet<TblMa> TblMas { get; set; } = null!;
        public virtual DbSet<TblMaterial> TblMaterials { get; set; } = null!;
        public virtual DbSet<TblMazu> TblMazus { get; set; } = null!;
        public virtual DbSet<TblMessraum> TblMessraums { get; set; } = null!;
        public virtual DbSet<TblMessraumAbarbeitung> TblMessraumAbarbeitungs { get; set; } = null!;
        public virtual DbSet<TblMm> TblMms { get; set; } = null!;
        public virtual DbSet<TblProjekt> TblProjekts { get; set; } = null!;
        public virtual DbSet<TblProjektAnhang> TblProjektAnhangs { get; set; } = null!;
        public virtual DbSet<TblRessKappa> TblRessKappas { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserCost> UserCosts { get; set; } = null!;
        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;
        public virtual DbSet<UserWorkArea> UserWorkAreas { get; set; } = null!;
        public virtual DbSet<Vorgang> Vorgangs { get; set; } = null!;
        public virtual DbSet<WorkArea> WorkAreas { get; set; } = null!;
        public virtual DbSet<WorkSap> WorkSaps { get; set; } = null!;

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DataContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-MEMDFDP\\SQLEXPRESS;Initial Catalog=DB_COS_LIEFERLISTE_SQL;Integrated Security=True;Trust Server Certificate=True;Command Timeout=300");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Costunit>(entity =>
            {
                entity.Property(e => e.CostunitId).ValueGeneratedNever();
            });

            modelBuilder.Entity<Machine>(entity =>
            {
                entity.HasOne(d => d.CostUnitNavigation)
                    .WithMany(p => p.Machines)
                    .HasForeignKey(d => d.CostUnit)
                    .HasConstraintName("FK_Machine_Costunit");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.PKey).IsFixedLength();
            });

            modelBuilder.Entity<PermissionRole>(entity =>
            {
                entity.HasKey(e => new { e.RoleKey, e.PermissionKey });

                entity.Property(e => e.PermissionKey).IsFixedLength();

                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.PermissionKeyNavigation)
                    .WithMany(p => p.PermissionRoles)
                    .HasForeignKey(d => d.PermissionKey)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionRoles_Permission");

                entity.HasOne(d => d.RoleKeyNavigation)
                    .WithMany(p => p.PermissionRoles)
                    .HasForeignKey(d => d.RoleKey)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermissionRoles_Roles");
            });

            modelBuilder.Entity<Ressource>(entity =>
            {
                entity.Property(e => e.WorkSapId).IsUnicode(false);

                entity.HasOne(d => d.WorkArea)
                    .WithMany(p => p.Ressources)
                    .HasForeignKey(d => d.WorkAreaId)
                    .HasConstraintName("FK_Ressource_WorkArea");
            });

            modelBuilder.Entity<RessourceCostUnit>(entity =>
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
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Created)
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Time of Create");

                entity.Property(e => e.Description).IsFixedLength();
            });

            modelBuilder.Entity<TblAbfrageblatt>(entity =>
            {
                entity.Property(e => e.Aid).IsUnicode(false);

                entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Aba)
                    .WithMany(p => p.TblAbfrageblatts)
                    .HasForeignKey(d => d.Abaid)
                    .HasConstraintName("FK_tblAbfrageblatt_tblAbfrageblattAuswahl");

                entity.HasOne(d => d.AidNavigation)
                    .WithMany(p => p.TblAbfrageblatts)
                    .HasForeignKey(d => d.Aid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblAbfrageblatt_tblAuftrag");
            });

            modelBuilder.Entity<TblAbfrageblattAuswahl>(entity =>
            {
                entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblArbeitsplatzZuteilung>(entity =>
            {
                entity.Property(e => e.Arbid).IsUnicode(false);

                entity.HasOne(d => d.BidNavigation)
                    .WithMany(p => p.TblArbeitsplatzZuteilungs)
                    .HasForeignKey(d => d.Bid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblArbeitsplatzZuteilung_tblbereich");
            });

            modelBuilder.Entity<TblAuftrag>(entity =>
            {
                entity.Property(e => e.Aid).IsUnicode(false);

                entity.Property(e => e.DummyMat).IsUnicode(false);

                entity.HasOne(d => d.DummyMatNavigation)
                    .WithMany(p => p.TblAuftrags)
                    .HasForeignKey(d => d.DummyMat)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblAuftrag_tblDummy1");

                entity.HasOne(d => d.MaterialNavigation)
                    .WithMany(p => p.TblAuftrags)
                    .HasForeignKey(d => d.Material)
                    .HasConstraintName("FK_tblAuftrag_tblMaterial");

                entity.HasOne(d => d.Pro)
                    .WithMany(p => p.TblAuftrags)
                    .HasForeignKey(d => d.ProId)
                    .HasConstraintName("FK_tblAuftrag_tblProjekt");
            });

            modelBuilder.Entity<TblDummy>(entity =>
            {
                entity.HasKey(e => e.Aid)
                    .HasName("PK_tblDummy_1");

                entity.Property(e => e.Aid).IsUnicode(false);
            });

            modelBuilder.Entity<TblEinstellTeil>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DummyMat).IsUnicode(false);

                entity.HasOne(d => d.DummyMatNavigation)
                    .WithMany(p => p.TblEinstellTeils)
                    .HasForeignKey(d => d.DummyMat)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblEinstellTeil_tblDummy");

                entity.HasOne(d => d.TtnrNavigation)
                    .WithMany(p => p.TblEinstellTeils)
                    .HasForeignKey(d => d.Ttnr)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblEinstellTeil_tblMaterial");
            });

            modelBuilder.Entity<TblEinstellteileTran>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TblGrunddaten>(entity =>
            {
                entity.Property(e => e.Gid).ValueGeneratedNever();
            });

            modelBuilder.Entity<TblMa>(entity =>
            {
                entity.Property(e => e.MaId).ValueGeneratedNever();
            });

            modelBuilder.Entity<TblMazu>(entity =>
            {
                entity.Property(e => e.MaZuId).ValueGeneratedNever();

                entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Ma)
                    .WithMany(p => p.TblMazus)
                    .HasForeignKey(d => d.MaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblMAZu_tblMA");
            });

            modelBuilder.Entity<TblProjektAnhang>(entity =>
            {
                entity.Property(e => e.Timestamp).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.PidNavigation)
                    .WithMany(p => p.TblProjektAnhangs)
                    .HasForeignKey(d => d.Pid)
                    .HasConstraintName("FK_tblProjektAnhang_tblProjekt");
            });

            modelBuilder.Entity<TblRessKappa>(entity =>
            {
                entity.Property(e => e.Id).HasComment("Identity");

                entity.Property(e => e.Created).HasComment("Timestamp to create the Datarow");

                entity.Property(e => e.End1)
                    .HasDefaultValueSql("((0))")
                    .HasComment("Ende Schicht 1");

                entity.Property(e => e.End2)
                    .HasDefaultValueSql("((0))")
                    .HasComment("Ende Schicht");

                entity.Property(e => e.End3)
                    .HasDefaultValueSql("((0))")
                    .HasComment("Ende Schicht 3");

                entity.Property(e => e.Rid).HasComment("Resource ID");

                entity.Property(e => e.Start1)
                    .HasDefaultValueSql("((0))")
                    .HasComment("Start Schicht 1");

                entity.Property(e => e.Start2)
                    .HasDefaultValueSql("((0))")
                    .HasComment("Start Schicht 2");

                entity.Property(e => e.Start3)
                    .HasDefaultValueSql("((0))")
                    .HasComment("Start Schicht 3");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserIdent)
                    .HasName("PK__User__1C1A74760950743A");

                entity.Property(e => e.Exited).HasDefaultValueSql("((0))");

                entity.Property(e => e.UsrName).IsUnicode(false);
            });

            modelBuilder.Entity<UserCost>(entity =>
            {
                entity.HasKey(e => new { e.UsrIdent, e.CostId });

                entity.HasOne(d => d.Cost)
                    .WithMany(p => p.UserCosts)
                    .HasForeignKey(d => d.CostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserCost_Costunit");

                entity.HasOne(d => d.UsrIdentNavigation)
                    .WithMany(p => p.UserCosts)
                    .HasForeignKey(d => d.UsrIdent)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_UserCost");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserIdent, e.RoleId });

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_UserRoles_Roles");

                entity.HasOne(d => d.UserIdentNavigation)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserIdent)
                    .HasConstraintName("FK_UserRoles_User");
            });

            modelBuilder.Entity<UserWorkArea>(entity =>
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
            });

            modelBuilder.Entity<Vorgang>(entity =>
            {
                entity.Property(e => e.VorgangId).IsUnicode(false);

                entity.Property(e => e.Aid).IsUnicode(false);

                entity.Property(e => e.ArbPlSap).IsUnicode(false);

                entity.Property(e => e.BemM).IsUnicode(false);

                entity.Property(e => e.BemMa).IsUnicode(false);

                entity.Property(e => e.BemT).IsUnicode(false);

                entity.Property(e => e.CommentMach).IsUnicode(false);

                entity.Property(e => e.Marker).IsFixedLength();

                entity.Property(e => e.ProcessingUom).IsFixedLength();

                entity.HasOne(d => d.AidNavigation)
                    .WithMany(p => p.Vorgangs)
                    .HasForeignKey(d => d.Aid)
                    .HasConstraintName("FK_tblVorgang_tblAuftrag");

                entity.HasOne(d => d.ArbPlSapNavigation)
                    .WithMany(p => p.Vorgangs)
                    .HasForeignKey(d => d.ArbPlSap)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Vorgang_WorkSap");

                entity.HasOne(d => d.RidNavigation)
                    .WithMany(p => p.Vorgangs)
                    .HasForeignKey(d => d.Rid)
                    .HasConstraintName("FK_Vorgang_Ressource");
            });

            modelBuilder.Entity<WorkSap>(entity =>
            {
                entity.Property(e => e.WorkSapId).IsUnicode(false);

                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Ressource)
                    .WithMany(p => p.WorkSaps)
                    .HasForeignKey(d => d.RessourceId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_WorkSap_Ressource");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
