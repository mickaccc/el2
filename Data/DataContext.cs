
using Lieferliste_WPF.Data.Configurations;
using Lieferliste_WPF.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
namespace Lieferliste_WPF.Data
{
    public partial class DataContext : DbContext
    {
        public virtual DbSet<Costunit> Costunits { get; set; } = null!;
        public virtual DbSet<Online> Onlines { get; set; } = null!;
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
        public DataContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
               
                optionsBuilder.UseSqlServer(Properties.Settings.Default.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configurations.CostunitConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.OnlineConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PermissionRoleConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RessourceConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RessourceCostUnitConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RoleConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TblAbfrageblattConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TblAbfrageblattAuswahlConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TblArbeitsplatzZuteilungConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TblAuftragConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TblDummyConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TblEinstellTeilConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TblEinstellteileTranConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TblGrunddatenConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TblMaConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TblMazuConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TblProjektAnhangConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TblRessKappaConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.UserCostConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.UserWorkAreaConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.VorgangConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.WorkSapConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
