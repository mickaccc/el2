using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lieferliste_WPF.Models;

public partial class DbCosLieferlisteSqlContext : DbContext
{
    public DbCosLieferlisteSqlContext()
    {
    }

    public DbCosLieferlisteSqlContext(DbContextOptions<DbCosLieferlisteSqlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auftrag> Auftrags { get; set; }

    public virtual DbSet<ChangedOrder> ChangedOrders { get; set; }

    public virtual DbSet<ChangedOrdersCo> ChangedOrdersCos { get; set; }

    public virtual DbSet<ErrorMessagesWebservice> ErrorMessagesWebservices { get; set; }

    public virtual DbSet<Lieferliste> Lieferlistes { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Messauftrag> Messauftrags { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PermissionRole> PermissionRoles { get; set; }

    public virtual DbSet<PerspectRole> PerspectRoles { get; set; }

    public virtual DbSet<PerspectUser> PerspectUsers { get; set; }

    public virtual DbSet<Perspective> Perspectives { get; set; }

    public virtual DbSet<ProductionOrderFilter> ProductionOrderFilters { get; set; }

    public virtual DbSet<ProductionOrdersCo> ProductionOrdersCos { get; set; }

    public virtual DbSet<ProductionOrdersFhm> ProductionOrdersFhms { get; set; }

    public virtual DbSet<RessZuteilView> RessZuteilViews { get; set; }

    public virtual DbSet<RessourceAllocation> RessourceAllocations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SapStatusDe> SapStatusDes { get; set; }

    public virtual DbSet<SapStatusEn> SapStatusEns { get; set; }

    public virtual DbSet<TblAbfrageblatt> TblAbfrageblatts { get; set; }

    public virtual DbSet<TblAbfrageblattAuswahl> TblAbfrageblattAuswahls { get; set; }

    public virtual DbSet<TblArbeitsplatzSap> TblArbeitsplatzSaps { get; set; }

    public virtual DbSet<TblArbeitsplatzZuteilung> TblArbeitsplatzZuteilungs { get; set; }

    public virtual DbSet<TblAuftrag> TblAuftrags { get; set; }

    public virtual DbSet<TblBerechtigung> TblBerechtigungs { get; set; }

    public virtual DbSet<TblDummy> TblDummies { get; set; }

    public virtual DbSet<TblEinstellTeil> TblEinstellTeils { get; set; }

    public virtual DbSet<TblEinstellteileTran> TblEinstellteileTrans { get; set; }

    public virtual DbSet<TblFeiertagSchliesstag> TblFeiertagSchliesstags { get; set; }

    public virtual DbSet<TblGrunddaten> TblGrunddatens { get; set; }

    public virtual DbSet<TblHoliMask> TblHoliMasks { get; set; }

    public virtual DbSet<TblMa> TblMas { get; set; }

    public virtual DbSet<TblMaterial> TblMaterials { get; set; }

    public virtual DbSet<TblMazu> TblMazus { get; set; }

    public virtual DbSet<TblMessraum> TblMessraums { get; set; }

    public virtual DbSet<TblMessraumAbarbeitung> TblMessraumAbarbeitungs { get; set; }

    public virtual DbSet<TblMm> TblMms { get; set; }

    public virtual DbSet<TblPause> TblPauses { get; set; }

    public virtual DbSet<TblProjekt> TblProjekts { get; set; }

    public virtual DbSet<TblProjektAnhang> TblProjektAnhangs { get; set; }

    public virtual DbSet<TblRessKappa> TblRessKappas { get; set; }

    public virtual DbSet<TblRessource> TblRessources { get; set; }

    public virtual DbSet<TblRessourceVorgang> TblRessourceVorgangs { get; set; }

    public virtual DbSet<TblUserBerechtigung> TblUserBerechtigungs { get; set; }

    public virtual DbSet<TblUserListe> TblUserListes { get; set; }

    public virtual DbSet<TblVorgang> TblVorgangs { get; set; }

    public virtual DbSet<TblVorgangAnhang> TblVorgangAnhangs { get; set; }

    public virtual DbSet<TblVorgangKlima> TblVorgangKlimas { get; set; }

    public virtual DbSet<TblVorgangMessraumDoku> TblVorgangMessraumDokus { get; set; }

    public virtual DbSet<Testchange> Testchanges { get; set; }

    public virtual DbSet<Testwebservice> Testwebservices { get; set; }

    public virtual DbSet<Tmpauftr> Tmpauftrs { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserSelectGrp> UserSelectGrps { get; set; }

    public virtual DbSet<UserWorkArea> UserWorkAreas { get; set; }

    public virtual DbSet<Vorgang> Vorgangs { get; set; }

    public virtual DbSet<WorkArea> WorkAreas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-88O2DD0;Initial Catalog=DB_COS_LIEFERLISTE_SQL;Integrated Security=True;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auftrag>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("auftrag");

            entity.Property(e => e.Abgeschlossen)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("abgeschlossen");
            entity.Property(e => e.Aid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AID");
            entity.Property(e => e.AuftragArt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AuftragFarbe)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ausgebl)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ausgebl");
            entity.Property(e => e.Bemerkung)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dringend)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DummyMat)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Eckende)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Eckstart)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fertig)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("fertig");
            entity.Property(e => e.Istende)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Iststart)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LieferTermin)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Mappe)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Material)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Prio)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ProID");
            entity.Property(e => e.Quantity)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SysStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Timestamp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<ChangedOrder>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.OrderNumber)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.StatusTabg).HasColumnName("StatusTABG");
            entity.Property(e => e.ZeitStempelAenderung).HasColumnType("datetime");
        });

        modelBuilder.Entity<ChangedOrdersCo>(entity =>
        {
            entity.HasKey(e => e.OrderNumber);

            entity.ToTable("ChangedOrdersCOS");

            entity.Property(e => e.OrderNumber)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.StatusTabg).HasColumnName("StatusTABG");
            entity.Property(e => e.ZeitStempelAenderung).HasColumnType("datetime");
        });

        modelBuilder.Entity<ErrorMessagesWebservice>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ErrorMessagesWEBSERVICE");

            entity.Property(e => e.ErrorMsg)
                .IsUnicode(false)
                .HasColumnName("errorMsg");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.Returnnr)
                .HasComment("100 = Order nicht vorhanden")
                .HasColumnName("returnnr");
            entity.Property(e => e.Time)
                .HasColumnType("datetime")
                .HasColumnName("time");
        });

        modelBuilder.Entity<Lieferliste>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("lieferliste");

            entity.Property(e => e.Abgeschlossen).HasColumnName("abgeschlossen");
            entity.Property(e => e.Aid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AID");
            entity.Property(e => e.ArbBereich).HasMaxLength(255);
            entity.Property(e => e.ArbBid).HasColumnName("ArbBID");
            entity.Property(e => e.ArbPlSap)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ArbPlSAP");
            entity.Property(e => e.Arbeitsplatz).HasMaxLength(80);
            entity.Property(e => e.AuftragArt).HasMaxLength(255);
            entity.Property(e => e.AuftragFarbe).HasMaxLength(10);
            entity.Property(e => e.Ausgebl).HasColumnName("ausgebl");
            entity.Property(e => e.BemM)
                .IsUnicode(false)
                .HasColumnName("Bem_M");
            entity.Property(e => e.BemMa)
                .IsUnicode(false)
                .HasColumnName("Bem_MA");
            entity.Property(e => e.BemT)
                .IsUnicode(false)
                .HasColumnName("Bem_T");
            entity.Property(e => e.Bemerkung).HasMaxLength(255);
            entity.Property(e => e.Fertig).HasColumnName("fertig");
            entity.Property(e => e.LieferTermin).HasMaxLength(255);
            entity.Property(e => e.Marker)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("marker");
            entity.Property(e => e.Material)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Pid).HasColumnName("PID");
            entity.Property(e => e.Plantermin).HasMaxLength(7);
            entity.Property(e => e.Prio).HasMaxLength(255);
            entity.Property(e => e.Projekt).HasMaxLength(50);
            entity.Property(e => e.ProjektArt).HasMaxLength(255);
            entity.Property(e => e.ProjektFarbe).HasMaxLength(10);
            entity.Property(e => e.QuantityMiss).HasColumnName("Quantity-miss");
            entity.Property(e => e.QuantityRework).HasColumnName("Quantity-rework");
            entity.Property(e => e.QuantityScrap).HasColumnName("Quantity-scrap");
            entity.Property(e => e.QuantityYield).HasColumnName("Quantity-yield");
            entity.Property(e => e.RessName).HasMaxLength(30);
            entity.Property(e => e.Rid).HasColumnName("RID");
            entity.Property(e => e.SpaetEnd).HasColumnType("datetime");
            entity.Property(e => e.SpaetStart).HasColumnType("datetime");
            entity.Property(e => e.SysStatus).HasMaxLength(255);
            entity.Property(e => e.Teil)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Termin).HasColumnType("datetime");
            entity.Property(e => e.Text).HasMaxLength(150);
            entity.Property(e => e.Vid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("VID");
            entity.Property(e => e.Vnr).HasColumnName("VNR");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.HostName).HasMaxLength(255);
            entity.Property(e => e.Level)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Logger)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Source)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Thread)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("material");

            entity.Property(e => e.Bezeichng)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ttnr)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TTNR");
        });

        modelBuilder.Entity<Messauftrag>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Messauftrag", "EMEA\\SCM2HL");

            entity.Property(e => e.Aid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AID");
            entity.Property(e => e.BemerkungMb).HasColumnName("Bemerkung_MB");
            entity.Property(e => e.BemerkungMt).HasColumnName("Bemerkung_Mt");
            entity.Property(e => e.KurztextVrg)
                .HasMaxLength(150)
                .HasColumnName("KurztextVRG");
            entity.Property(e => e.LfndProzess).HasColumnName("lfndProzess");
            entity.Property(e => e.MaId).HasColumnName("MaID");
            entity.Property(e => e.Material)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Materialkurztext)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RessName).HasMaxLength(30);
            entity.Property(e => e.Rid).HasColumnName("RID");
            entity.Property(e => e.Sonstiges).HasColumnName("sonstiges");
            entity.Property(e => e.Timestamp)
                .HasMaxLength(50)
                .HasColumnName("timestamp");
            entity.Property(e => e.UserIdent).HasMaxLength(50);
            entity.Property(e => e.Vid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("VID");
            entity.Property(e => e.WunschDatum).HasMaxLength(50);
            entity.Property(e => e.WunschZeit).HasMaxLength(50);
            entity.Property(e => e.Zustand)
                .HasMaxLength(50)
                .HasColumnName("zustand");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Berechtigung);

            entity.Property(e => e.Berechtigung).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(50);
        });

        modelBuilder.Entity<PermissionRole>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created");
            entity.Property(e => e.PermissionKey).HasMaxLength(255);
            entity.Property(e => e.UserKey).HasMaxLength(255);

            entity.HasOne(d => d.PermissionKeyNavigation).WithMany(p => p.PermissionRoles)
                .HasForeignKey(d => d.PermissionKey)
                .HasConstraintName("FK_PermissionRoles_tblBerechtigung");

            entity.HasOne(d => d.RoleKeyNavigation).WithMany(p => p.PermissionRoles)
                .HasForeignKey(d => d.RoleKey)
                .HasConstraintName("FK_PermissionRoles_Roles");
        });

        modelBuilder.Entity<PerspectRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Perspect__3214EC07BFE1E70B");

            entity.ToTable("PerspectRole");

            entity.Property(e => e.Modified).HasMaxLength(50);
            entity.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Modified-date");
            entity.Property(e => e.PerspectId).HasColumnName("PerspectID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Perspect).WithMany(p => p.PerspectRoles)
                .HasForeignKey(d => d.PerspectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PerspectRole_Perspective");

            entity.HasOne(d => d.Role).WithMany(p => p.PerspectRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PerspectRole_Roles");
        });

        modelBuilder.Entity<PerspectUser>(entity =>
        {
            entity.HasKey(e => e.PerUsrId);

            entity.ToTable("PerspectUser");

            entity.Property(e => e.PerUsrId).HasColumnName("PerUsrID");
            entity.Property(e => e.PerspId).HasColumnName("PerspID");
            entity.Property(e => e.UsrId)
                .HasMaxLength(255)
                .HasColumnName("UsrID");

            entity.HasOne(d => d.Persp).WithMany(p => p.PerspectUsers)
                .HasForeignKey(d => d.PerspId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PerspectUser_Perspective");

            entity.HasOne(d => d.Usr).WithMany(p => p.PerspectUsers)
                .HasForeignKey(d => d.UsrId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PerspectUser_tblUserListe");
        });

        modelBuilder.Entity<Perspective>(entity =>
        {
            entity.HasKey(e => e.PerspectId);

            entity.ToTable("Perspective");

            entity.Property(e => e.PerspectId).HasColumnName("PerspectID");
            entity.Property(e => e.PerspectFileName).HasMaxLength(50);
            entity.Property(e => e.PerspectName).HasMaxLength(50);
        });

        modelBuilder.Entity<ProductionOrderFilter>(entity =>
        {
            entity.HasKey(e => e.OrderNumber);

            entity.ToTable("ProductionOrderFilter");

            entity.Property(e => e.OrderNumber).HasMaxLength(32);
            entity.Property(e => e.Kommentar).HasMaxLength(50);
            entity.Property(e => e.ZeitStempelÄnderung).HasColumnType("smalldatetime");
        });

        modelBuilder.Entity<ProductionOrdersCo>(entity =>
        {
            entity.HasKey(e => new { e.OrderNumber, e.OperationNumber });

            entity.ToTable("ProductionOrdersCOS");

            entity.Property(e => e.OrderNumber)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ActualExecutionFinishDate).HasColumnType("datetime");
            entity.Property(e => e.ActualExecutionFinishTime)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ActualExecutionStartDate).HasColumnType("datetime");
            entity.Property(e => e.ActualExecutionStartTime)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ActualFinishDate).HasColumnType("datetime");
            entity.Property(e => e.ActualStartDate).HasColumnType("datetime");
            entity.Property(e => e.ControlKey)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Idnr)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("IDNR");
            entity.Property(e => e.MaterialDesc)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaterialNumber)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OperationShortText)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ProcessingTimeUom)
                .HasMaxLength(16)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ProcessingTimeUOM");
            entity.Property(e => e.ProductionOrderHeaderStatus)
                .HasMaxLength(128)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ProductionOrderStatusForProcess)
                .HasMaxLength(128)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ScheduledExecutionFinishDate).HasColumnType("datetime");
            entity.Property(e => e.ScheduledExecutionFinishTime)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ScheduledExecutionStartDate).HasColumnType("datetime");
            entity.Property(e => e.ScheduledExecutionStartTime)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ScheduledFinish).HasColumnType("datetime");
            entity.Property(e => e.ScheduledStartDate).HasColumnType("datetime");
            entity.Property(e => e.ZeitStempelAenderung).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProductionOrdersFhm>(entity =>
        {
            entity.HasKey(e => new { e.OrderNumber, e.OperationNumber });

            entity.ToTable("ProductionOrdersFHM");

            entity.Property(e => e.OrderNumber)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Idnr)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("IDNR");
            entity.Property(e => e.MaterialDesc)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaterialNumber)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OperationShortText)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ProcessesExtension1Sw)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ProcessesExtension1SW");
            entity.Property(e => e.ProductionOrderStatusForProcess)
                .HasMaxLength(128)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ZeitStempelAenderung).HasColumnType("datetime");
        });

        modelBuilder.Entity<RessZuteilView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("RessZuteilView");

            entity.Property(e => e.Abteilung).HasMaxLength(255);
            entity.Property(e => e.Aid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AID");
            entity.Property(e => e.Arbid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ARBID");
            entity.Property(e => e.Arbzutid).HasColumnName("ARBZUTID");
            entity.Property(e => e.Beaze).HasColumnName("BEAZE");
            entity.Property(e => e.BemM)
                .IsUnicode(false)
                .HasColumnName("Bem_M");
            entity.Property(e => e.BemMa)
                .IsUnicode(false)
                .HasColumnName("Bem_MA");
            entity.Property(e => e.BemT)
                .IsUnicode(false)
                .HasColumnName("Bem_T");
            entity.Property(e => e.Bereich).HasMaxLength(255);
            entity.Property(e => e.Bezeichnung).HasMaxLength(80);
            entity.Property(e => e.Bid).HasColumnName("BID");
            entity.Property(e => e.Info).HasMaxLength(255);
            entity.Property(e => e.Inventarnummer).HasMaxLength(255);
            entity.Property(e => e.Korrect).HasColumnName("korrect");
            entity.Property(e => e.Material)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MaterialDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.QuantityMiss).HasColumnName("Quantity-miss");
            entity.Property(e => e.QuantityRework).HasColumnName("Quantity-rework");
            entity.Property(e => e.QuantityScrap).HasColumnName("Quantity-scrap");
            entity.Property(e => e.QuantityYield).HasColumnName("Quantity-yield");
            entity.Property(e => e.RessName).HasMaxLength(30);
            entity.Property(e => e.Rid).HasColumnName("RID");
            entity.Property(e => e.Sort).HasColumnName("SORT");
            entity.Property(e => e.SpaetEnd).HasColumnType("datetime");
            entity.Property(e => e.Spos).HasColumnName("SPOS");
            entity.Property(e => e.Text).HasMaxLength(150);
            entity.Property(e => e.Vgrid).HasColumnName("VGRID");
            entity.Property(e => e.Vid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("VID");
            entity.Property(e => e.Vnr).HasColumnName("VNR");
        });

        modelBuilder.Entity<RessourceAllocation>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("RessourceAllocation");

            entity.Property(e => e.Abteilung).HasMaxLength(15);
            entity.Property(e => e.Aid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AID");
            entity.Property(e => e.ArbPlSap)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ArbPlSAP");
            entity.Property(e => e.Beaze).HasColumnName("BEAZE");
            entity.Property(e => e.BemM)
                .IsUnicode(false)
                .HasColumnName("Bem_M");
            entity.Property(e => e.BemMa)
                .IsUnicode(false)
                .HasColumnName("Bem_MA");
            entity.Property(e => e.BemT)
                .IsUnicode(false)
                .HasColumnName("Bem_T");
            entity.Property(e => e.Bereich).HasMaxLength(255);
            entity.Property(e => e.Bid).HasColumnName("BID");
            entity.Property(e => e.Eckende).HasColumnType("datetime");
            entity.Property(e => e.Eckstart).HasColumnType("datetime");
            entity.Property(e => e.Korrect).HasColumnName("korrect");
            entity.Property(e => e.Material)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MaterialDescription)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.QuantityMiss).HasColumnName("Quantity-miss");
            entity.Property(e => e.QuantityYield).HasColumnName("Quantity-yield");
            entity.Property(e => e.RessName).HasMaxLength(30);
            entity.Property(e => e.Rid).HasColumnName("RID");
            entity.Property(e => e.SpaetEnd).HasColumnType("datetime");
            entity.Property(e => e.Spos).HasColumnName("SPOS");
            entity.Property(e => e.Text).HasMaxLength(150);
            entity.Property(e => e.Usr)
                .HasMaxLength(10)
                .HasColumnName("usr");
            entity.Property(e => e.Vid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("VID");
            entity.Property(e => e.Vnr).HasColumnName("VNR");
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
        });

        modelBuilder.Entity<SapStatusDe>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SAP-Status-de");

            entity.Property(e => e.Stat).HasMaxLength(10);
            entity.Property(e => e.Status).HasMaxLength(256);
            entity.Property(e => e.SysStat).HasMaxLength(10);
        });

        modelBuilder.Entity<SapStatusEn>(entity =>
        {
            entity.HasKey(e => e.SysStat);

            entity.ToTable("SAP-Status-en");

            entity.Property(e => e.SysStat).HasMaxLength(10);
            entity.Property(e => e.Description).HasMaxLength(256);
            entity.Property(e => e.Stat).HasMaxLength(10);
        });

        modelBuilder.Entity<TblAbfrageblatt>(entity =>
        {
            entity.HasKey(e => e.Abid);

            entity.ToTable("tblAbfrageblatt");

            entity.Property(e => e.Abid).HasColumnName("ABID");
            entity.Property(e => e.Abaid).HasColumnName("ABAID");
            entity.Property(e => e.Aid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AID");
            entity.Property(e => e.Feld13).HasColumnType("datetime");
            entity.Property(e => e.Feld17).HasColumnType("datetime");
            entity.Property(e => e.Feld9).HasColumnType("datetime");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.Aba).WithMany(p => p.TblAbfrageblatts)
                .HasForeignKey(d => d.Abaid)
                .HasConstraintName("FK_tblAbfrageblatt_tblAbfrageblattAuswahl");

            entity.HasOne(d => d.AidNavigation).WithMany(p => p.TblAbfrageblatts)
                .HasForeignKey(d => d.Aid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblAbfrageblatt_tblAuftrag");
        });

        modelBuilder.Entity<TblAbfrageblattAuswahl>(entity =>
        {
            entity.HasKey(e => e.Abaid);

            entity.ToTable("tblAbfrageblattAuswahl");

            entity.Property(e => e.Abaid).HasColumnName("ABAID");
            entity.Property(e => e.Aktuell).HasColumnName("aktuell");
            entity.Property(e => e.Feld1).HasMaxLength(255);
            entity.Property(e => e.Feld10).HasMaxLength(255);
            entity.Property(e => e.Feld14).HasMaxLength(255);
            entity.Property(e => e.Feld18).HasMaxLength(255);
            entity.Property(e => e.Feld19).HasMaxLength(255);
            entity.Property(e => e.Feld2).HasMaxLength(255);
            entity.Property(e => e.Feld21).HasMaxLength(255);
            entity.Property(e => e.Feld4).HasMaxLength(255);
            entity.Property(e => e.Felder).HasMaxLength(255);
            entity.Property(e => e.FelderBlau)
                .HasMaxLength(255)
                .HasColumnName("Felder_Blau");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<TblArbeitsplatzSap>(entity =>
        {
            entity.HasKey(e => e.Arbid);

            entity.ToTable("tblArbeitsplatzSAP");

            entity.Property(e => e.Arbid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ARBID");
            entity.Property(e => e.Bezeichnung).HasMaxLength(80);
            entity.Property(e => e.Rid)
                .HasDefaultValueSql("((0))")
                .HasColumnName("RID");
        });

        modelBuilder.Entity<TblArbeitsplatzZuteilung>(entity =>
        {
            entity.HasKey(e => e.Arbzutid);

            entity.ToTable("tblArbeitsplatzZuteilung");

            entity.Property(e => e.Arbzutid).HasColumnName("ARBZUTID");
            entity.Property(e => e.Arbid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ARBID");
            entity.Property(e => e.Bid).HasColumnName("BID");
            entity.Property(e => e.Rid).HasColumnName("RID");
            entity.Property(e => e.ZutName).HasMaxLength(255);

            entity.HasOne(d => d.Arb).WithMany(p => p.TblArbeitsplatzZuteilungs)
                .HasForeignKey(d => d.Arbid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblArbeitsplatzZuteilung_tblArbeitsplatzSAP");

            entity.HasOne(d => d.BidNavigation).WithMany(p => p.TblArbeitsplatzZuteilungs)
                .HasForeignKey(d => d.Bid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblArbeitsplatzZuteilung_tblbereich");

            entity.HasOne(d => d.RidNavigation).WithMany(p => p.TblArbeitsplatzZuteilungs)
                .HasForeignKey(d => d.Rid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblArbeitsplatzZuteilung_tblRessource");
        });

        modelBuilder.Entity<TblAuftrag>(entity =>
        {
            entity.HasKey(e => e.Aid);

            entity.ToTable("tblAuftrag");

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
            entity.Property(e => e.Prio).HasMaxLength(255);
            entity.Property(e => e.ProId).HasColumnName("ProID");
            entity.Property(e => e.SysStatus).HasMaxLength(255);
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.DummyMatNavigation).WithMany(p => p.TblAuftrags)
                .HasForeignKey(d => d.DummyMat)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblAuftrag_tblDummy1");

            entity.HasOne(d => d.MaterialNavigation).WithMany(p => p.TblAuftrags)
                .HasForeignKey(d => d.Material)
                .HasConstraintName("FK_tblAuftrag_tblMaterial");

            entity.HasOne(d => d.Pro).WithMany(p => p.TblAuftrags)
                .HasForeignKey(d => d.ProId)
                .HasConstraintName("FK_tblAuftrag_tblProjekt");
        });

        modelBuilder.Entity<TblBerechtigung>(entity =>
        {
            entity.HasKey(e => e.Berechtigung);

            entity.ToTable("tblBerechtigung");

            entity.Property(e => e.Berechtigung).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(50);
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

        modelBuilder.Entity<TblEinstellTeil>(entity =>
        {
            entity.HasKey(e => e.EinstId);

            entity.ToTable("tblEinstellTeil");

            entity.Property(e => e.EinstId).HasColumnName("EinstID");
            entity.Property(e => e.Aid)
                .HasMaxLength(50)
                .HasColumnName("AID");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created");
            entity.Property(e => e.DummyMat)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastModifed)
                .HasColumnType("datetime")
                .HasColumnName("lastModifed");
            entity.Property(e => e.Ttnr)
                .HasMaxLength(255)
                .HasColumnName("TTNR");
            entity.Property(e => e.Verschrottet).HasColumnName("verschrottet");

            entity.HasOne(d => d.DummyMatNavigation).WithMany(p => p.TblEinstellTeils)
                .HasForeignKey(d => d.DummyMat)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblEinstellTeil_tblDummy");

            entity.HasOne(d => d.TtnrNavigation).WithMany(p => p.TblEinstellTeils)
                .HasForeignKey(d => d.Ttnr)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblEinstellTeil_tblMaterial");
        });

        modelBuilder.Entity<TblEinstellteileTran>(entity =>
        {
            entity.HasKey(e => e.TransId);

            entity.ToTable("tblEinstellteileTrans");

            entity.Property(e => e.TransId).HasColumnName("TransID");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created");
            entity.Property(e => e.EinstId).HasColumnName("EinstID");
            entity.Property(e => e.Pnr).HasColumnName("PNR");
            entity.Property(e => e.Stk).HasColumnName("stk");
            entity.Property(e => e.TransArt).HasMaxLength(10);
            entity.Property(e => e.Vid)
                .HasMaxLength(255)
                .HasColumnName("VID");
        });

        modelBuilder.Entity<TblFeiertagSchliesstag>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tblFeiertagSchliesstag");

            entity.Property(e => e.Bemerkung).HasMaxLength(255);
            entity.Property(e => e.Datum).HasColumnType("datetime");
            entity.Property(e => e.Fesid)
                .ValueGeneratedOnAdd()
                .HasColumnName("FESID");
        });

        modelBuilder.Entity<TblGrunddaten>(entity =>
        {
            entity.HasKey(e => e.Gid);

            entity.ToTable("tblGrunddaten");

            entity.Property(e => e.Gid)
                .ValueGeneratedNever()
                .HasColumnName("GID");
            entity.Property(e => e.Beschreibung).HasMaxLength(255);
            entity.Property(e => e.Kuerzel).HasMaxLength(50);
            entity.Property(e => e.Text).HasMaxLength(50);
            entity.Property(e => e.Wert).HasMaxLength(255);
        });

        modelBuilder.Entity<TblHoliMask>(entity =>
        {
            entity.HasKey(e => e.Hid);

            entity.ToTable("tblHoliMask");

            entity.Property(e => e.Hid).HasColumnName("_hid");
            entity.Property(e => e.HoliDate)
                .HasColumnType("datetime")
                .HasColumnName("holiDate");
            entity.Property(e => e.HoliDescript)
                .HasMaxLength(255)
                .HasColumnName("holiDescript");
            entity.Property(e => e.HoliFormula)
                .HasMaxLength(255)
                .HasColumnName("holiFormula");
            entity.Property(e => e.HoliName)
                .HasMaxLength(255)
                .HasColumnName("holiName");
        });

        modelBuilder.Entity<TblMa>(entity =>
        {
            entity.HasKey(e => e.MaId);

            entity.ToTable("tblMA");

            entity.Property(e => e.MaId)
                .ValueGeneratedNever()
                .HasColumnName("MaID");
            entity.Property(e => e.BemerkungMb).HasColumnName("Bemerkung_MB");
            entity.Property(e => e.BemerkungMt).HasColumnName("Bemerkung_Mt");
            entity.Property(e => e.LfndProzess).HasColumnName("lfndProzess");
            entity.Property(e => e.Rid).HasColumnName("RID");
            entity.Property(e => e.Sonstiges).HasColumnName("sonstiges");
            entity.Property(e => e.Timestamp)
                .HasMaxLength(50)
                .HasColumnName("timestamp");
            entity.Property(e => e.UserIdent).HasMaxLength(50);
            entity.Property(e => e.Vid)
                .HasMaxLength(255)
                .HasColumnName("VID");
            entity.Property(e => e.WunschDatum).HasMaxLength(50);
            entity.Property(e => e.WunschZeit).HasMaxLength(50);
            entity.Property(e => e.Zustand)
                .HasMaxLength(50)
                .HasColumnName("zustand");
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

        modelBuilder.Entity<TblMazu>(entity =>
        {
            entity.HasKey(e => e.MaZuId);

            entity.ToTable("tblMAZu");

            entity.Property(e => e.MaZuId)
                .ValueGeneratedNever()
                .HasColumnName("MaZuID");
            entity.Property(e => e.MaId).HasColumnName("MaID");
            entity.Property(e => e.MmId).HasColumnName("MmID");
            entity.Property(e => e.Mzeit).HasColumnName("MZeit");
            entity.Property(e => e.Rzeit).HasColumnName("RZeit");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.Ma).WithMany(p => p.TblMazus)
                .HasForeignKey(d => d.MaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblMAZu_tblMA");
        });

        modelBuilder.Entity<TblMessraum>(entity =>
        {
            entity.HasKey(e => e.Mrid);

            entity.ToTable("tblMessraum");

            entity.Property(e => e.Mrid).HasColumnName("MRID");
            entity.Property(e => e.AbarbId).HasColumnName("AbarbID");
            entity.Property(e => e.Erledigt).HasColumnName("erledigt");
            entity.Property(e => e.Msf).HasColumnName("MSF");
            entity.Property(e => e.MsfEnde)
                .HasColumnType("datetime")
                .HasColumnName("MSF_Ende");
            entity.Property(e => e.MsfInfo).HasColumnName("MSF_INFO");
            entity.Property(e => e.MsfUserEnde)
                .HasMaxLength(255)
                .HasColumnName("MSF_User_Ende");
            entity.Property(e => e.MtEnde)
                .HasColumnType("datetime")
                .HasColumnName("MT_Ende");
            entity.Property(e => e.MtInfo).HasColumnName("MT_INFO");
            entity.Property(e => e.MtStart)
                .HasColumnType("datetime")
                .HasColumnName("MT_Start");
            entity.Property(e => e.MtUserEnde)
                .HasMaxLength(10)
                .HasColumnName("MT_User_Ende");
            entity.Property(e => e.MtUserStart)
                .HasMaxLength(10)
                .HasColumnName("MT_User_Start");
            entity.Property(e => e.Vid)
                .HasMaxLength(255)
                .HasColumnName("VID");
            entity.Property(e => e.VorgMrid).HasColumnName("VorgMRID");
        });

        modelBuilder.Entity<TblMessraumAbarbeitung>(entity =>
        {
            entity.HasKey(e => e.AbarbId);

            entity.ToTable("tblMessraumAbarbeitung");

            entity.Property(e => e.AbarbId).HasColumnName("AbarbID");
            entity.Property(e => e.Info).HasMaxLength(255);
        });

        modelBuilder.Entity<TblMm>(entity =>
        {
            entity.HasKey(e => e.MmId);

            entity.ToTable("tblMm");

            entity.Property(e => e.MmId).HasColumnName("MmID");
            entity.Property(e => e.Messmaschine).HasMaxLength(50);
        });

        modelBuilder.Entity<TblPause>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tblPause");

            entity.Property(e => e.Bemerkung).HasMaxLength(255);
            entity.Property(e => e.PauseId)
                .ValueGeneratedOnAdd()
                .HasColumnName("PauseID");
        });

        modelBuilder.Entity<TblProjekt>(entity =>
        {
            entity.HasKey(e => e.Pid);

            entity.ToTable("tblProjekt");

            entity.Property(e => e.Pid).HasColumnName("PID");
            entity.Property(e => e.Projekt).HasMaxLength(50);
            entity.Property(e => e.ProjektArt).HasMaxLength(255);
            entity.Property(e => e.ProjektFarbe).HasMaxLength(10);
        });

        modelBuilder.Entity<TblProjektAnhang>(entity =>
        {
            entity.HasKey(e => e.Panhid);

            entity.ToTable("tblProjektAnhang");

            entity.Property(e => e.Panhid).HasColumnName("PANHID");
            entity.Property(e => e.Aktuell).HasColumnName("aktuell");
            entity.Property(e => e.AnhangInfo).HasMaxLength(50);
            entity.Property(e => e.Dateiname).HasMaxLength(255);
            entity.Property(e => e.Pid).HasColumnName("PID");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.UserIdent).HasMaxLength(50);

            entity.HasOne(d => d.PidNavigation).WithMany(p => p.TblProjektAnhangs)
                .HasForeignKey(d => d.Pid)
                .HasConstraintName("FK_tblProjektAnhang_tblProjekt");
        });

        modelBuilder.Entity<TblRessKappa>(entity =>
        {
            entity.ToTable("tblRessKappa");

            entity.Property(e => e.Id)
                .HasComment("Identity")
                .HasColumnName("ID");
            entity.Property(e => e.Comment1).HasColumnName("comment1");
            entity.Property(e => e.Comment2).HasColumnName("comment2");
            entity.Property(e => e.Comment3).HasColumnName("comment3");
            entity.Property(e => e.Created)
                .HasComment("Timestamp to create the Datarow")
                .HasColumnType("datetime");
            entity.Property(e => e.Datum).HasColumnType("date");
            entity.Property(e => e.End1)
                .HasDefaultValueSql("((0))")
                .HasComment("Ende Schicht 1")
                .HasColumnName("end1");
            entity.Property(e => e.End2)
                .HasDefaultValueSql("((0))")
                .HasComment("Ende Schicht")
                .HasColumnName("end2");
            entity.Property(e => e.End3)
                .HasDefaultValueSql("((0))")
                .HasComment("Ende Schicht 3")
                .HasColumnName("end3");
            entity.Property(e => e.Rid)
                .HasComment("Resource ID")
                .HasColumnName("RID");
            entity.Property(e => e.Start1)
                .HasDefaultValueSql("((0))")
                .HasComment("Start Schicht 1")
                .HasColumnName("start1");
            entity.Property(e => e.Start2)
                .HasDefaultValueSql("((0))")
                .HasComment("Start Schicht 2")
                .HasColumnName("start2");
            entity.Property(e => e.Start3)
                .HasDefaultValueSql("((0))")
                .HasComment("Start Schicht 3")
                .HasColumnName("start3");
            entity.Property(e => e.Updated)
                .HasColumnType("datetime")
                .HasColumnName("updated");
        });

        modelBuilder.Entity<TblRessource>(entity =>
        {
            entity.HasKey(e => e.Rid);

            entity.ToTable("tblRessource");

            entity.Property(e => e.Rid).HasColumnName("RID");
            entity.Property(e => e.Abteilung).HasMaxLength(15);
            entity.Property(e => e.Info).HasMaxLength(255);
            entity.Property(e => e.Inventarnummer).HasMaxLength(255);
            entity.Property(e => e.RessName).HasMaxLength(30);
        });

        modelBuilder.Entity<TblRessourceVorgang>(entity =>
        {
            entity.HasKey(e => e.Vgrid);

            entity.ToTable("tblRessourceVorgang");

            entity.Property(e => e.Vgrid).HasColumnName("VGRID");
            entity.Property(e => e.DateCalculated).HasColumnType("datetime");
            entity.Property(e => e.Korrect).HasColumnName("korrect");
            entity.Property(e => e.Kw).HasColumnName("KW");
            entity.Property(e => e.Rid).HasColumnName("RID");
            entity.Property(e => e.Spos).HasColumnName("SPOS");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.Usr)
                .HasMaxLength(10)
                .HasColumnName("usr");
            entity.Property(e => e.Vid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("VID");

            entity.HasOne(d => d.RidNavigation).WithMany(p => p.TblRessourceVorgangs)
                .HasForeignKey(d => d.Rid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblRessourceVorgang_tblRessource");

            entity.HasOne(d => d.VidNavigation).WithMany(p => p.TblRessourceVorgangs)
                .HasForeignKey(d => d.Vid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblRessourceVorgang_tblVorgang");
        });

        modelBuilder.Entity<TblUserBerechtigung>(entity =>
        {
            entity.ToTable("tblUserBerechtigung");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Ablaufdatum)
                .HasColumnType("datetime")
                .HasColumnName("ablaufdatum");
            entity.Property(e => e.Berid)
                .HasMaxLength(255)
                .HasColumnName("BERID");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.UserIdent).HasMaxLength(255);
        });

        modelBuilder.Entity<TblUserListe>(entity =>
        {
            entity.HasKey(e => e.UserIdent);

            entity.ToTable("tblUserListe");

            entity.Property(e => e.UserIdent).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Exited).HasColumnName("exited");
            entity.Property(e => e.InfoAnzeigen).HasColumnName("info_anzeigen");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Personalnummer).HasMaxLength(50);
        });

        modelBuilder.Entity<TblVorgang>(entity =>
        {
            entity.HasKey(e => e.Vid);

            entity.ToTable("tblVorgang");

            entity.Property(e => e.Vid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("VID");
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
            entity.Property(e => e.Ausgebl).HasColumnName("ausgebl");
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
            entity.Property(e => e.Bid).HasColumnName("BID");
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
            entity.Property(e => e.Rstze).HasColumnName("RSTZE");
            entity.Property(e => e.RstzeEinheit)
                .HasMaxLength(5)
                .HasColumnName("RSTZE_Einheit");
            entity.Property(e => e.SpaetEnd).HasColumnType("datetime");
            entity.Property(e => e.SpaetStart).HasColumnType("datetime");
            entity.Property(e => e.SteuSchl).HasMaxLength(255);
            entity.Property(e => e.SysStatus).HasMaxLength(255);
            entity.Property(e => e.Termin).HasColumnType("datetime");
            entity.Property(e => e.Text).HasMaxLength(150);
            entity.Property(e => e.Vnr).HasColumnName("VNR");
            entity.Property(e => e.Wrtze).HasColumnName("WRTZE");
            entity.Property(e => e.WrtzeEinheit)
                .HasMaxLength(5)
                .HasColumnName("WRTZE_Einheit");

            entity.HasOne(d => d.AidNavigation).WithMany(p => p.TblVorgangs)
                .HasForeignKey(d => d.Aid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblVorgang_tblAuftrag");

            entity.HasOne(d => d.ArbPlSapNavigation).WithMany(p => p.TblVorgangs)
                .HasForeignKey(d => d.ArbPlSap)
                .HasConstraintName("FK_tblVorgang_tblArbeitsplatzSAP");
        });

        modelBuilder.Entity<TblVorgangAnhang>(entity =>
        {
            entity.HasKey(e => e.Vanhid);

            entity.ToTable("tblVorgangAnhang");

            entity.Property(e => e.Vanhid)
                .ValueGeneratedNever()
                .HasColumnName("VANHID");
            entity.Property(e => e.Aktuell).HasColumnName("aktuell");
            entity.Property(e => e.AnhangInfo).HasMaxLength(50);
            entity.Property(e => e.Bereich).HasMaxLength(50);
            entity.Property(e => e.Dateiname).HasMaxLength(255);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.UserIdent).HasMaxLength(50);
            entity.Property(e => e.Vid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("VID");

            entity.HasOne(d => d.VidNavigation).WithMany(p => p.TblVorgangAnhangs)
                .HasForeignKey(d => d.Vid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblVorgangAnhang_tblVorgang");
        });

        modelBuilder.Entity<TblVorgangKlima>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tblVorgangKlima");

            entity.Property(e => e.AnzahlDruck).HasColumnName("Anzahl_Druck");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.Vid)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("VID");
            entity.Property(e => e.VorgKlimaId).HasColumnName("VorgKlimaID");
        });

        modelBuilder.Entity<TblVorgangMessraumDoku>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tblVorgangMessraumDoku");

            entity.Property(e => e.Dateiname).HasMaxLength(255);
            entity.Property(e => e.Info).HasMaxLength(255);
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.UserIdent).HasMaxLength(255);
            entity.Property(e => e.Vid).HasColumnName("VID");
            entity.Property(e => e.Vmdid).HasColumnName("VMDID");
        });

        modelBuilder.Entity<Testchange>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("testchange");

            entity.Property(e => e.OrderNo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Testwebservice>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("TESTWEBSERVICE");

            entity.Property(e => e.ActualExecutionFinishDate).HasColumnType("datetime");
            entity.Property(e => e.ActualExecutionFinishTime)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ActualExecutionStartDate).HasColumnType("datetime");
            entity.Property(e => e.ActualExecutionStartTime)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ActualFinishDate).HasColumnType("datetime");
            entity.Property(e => e.ActualStartDate).HasColumnType("datetime");
            entity.Property(e => e.ControlKey)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Idnr)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("IDNR");
            entity.Property(e => e.MaterialDesc)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaterialNumber)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OperationShortText)
                .HasMaxLength(256)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(32)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ProcessingTimeUom)
                .HasMaxLength(16)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ProcessingTimeUOM");
            entity.Property(e => e.ProductionOrderHeaderStatus)
                .HasMaxLength(128)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ProductionOrderStatusForProcess)
                .HasMaxLength(128)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ScheduledExecutionFinishDate).HasColumnType("datetime");
            entity.Property(e => e.ScheduledExecutionFinishTime)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ScheduledExecutionStartDate).HasColumnType("datetime");
            entity.Property(e => e.ScheduledExecutionStartTime)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ScheduledFinish).HasColumnType("datetime");
            entity.Property(e => e.ScheduledStartDate).HasColumnType("datetime");
            entity.Property(e => e.ZeitStempelAenderung).HasColumnType("datetime");
        });

        modelBuilder.Entity<Tmpauftr>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tmpauftr");

            entity.Property(e => e.Aaid)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("aaid");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserIdent).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.UserIdentNavigation).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserIdent)
                .HasConstraintName("FK_UserRoles_tblUserListe");
        });

        modelBuilder.Entity<UserSelectGrp>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("UserSelectGrp", "EMEA\\SCM2HL");

            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UserIdent).HasMaxLength(255);
        });

        modelBuilder.Entity<UserWorkArea>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserUnion");

            entity.ToTable("UserWorkArea");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BerId).HasColumnName("BerID");
            entity.Property(e => e.UserId)
                .HasMaxLength(255)
                .HasColumnName("UserID");
        });

        modelBuilder.Entity<Vorgang>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("vorgang");

            entity.Property(e => e.ActualEndDate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ActualStartDate)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Aid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("AID");
            entity.Property(e => e.Aktuell)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("aktuell");
            entity.Property(e => e.ArbPlSap)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ArbPlSAP");
            entity.Property(e => e.Ausgebl)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ausgebl");
            entity.Property(e => e.BasisMg)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Beaze)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BEAZE");
            entity.Property(e => e.BeazeEinheit)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BEAZE_Einheit");
            entity.Property(e => e.BemM)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Bem_M");
            entity.Property(e => e.BemMa)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Bem_MA");
            entity.Property(e => e.BemT)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Bem_T");
            entity.Property(e => e.Bid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BID");
            entity.Property(e => e.Marker)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("marker");
            entity.Property(e => e.ProcessTime)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProcessingUom)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ProcessingUOM");
            entity.Property(e => e.QuantityMiss)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Quantity-miss");
            entity.Property(e => e.QuantityRework)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Quantity-rework");
            entity.Property(e => e.QuantityScrap)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Quantity-scrap");
            entity.Property(e => e.QuantityYield)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Quantity-yield");
            entity.Property(e => e.Rstze)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RSTZE");
            entity.Property(e => e.RstzeEinheit)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RSTZE_Einheit");
            entity.Property(e => e.SpaetEnd)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SpaetStart)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SteuSchl)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SysStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Termin)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Text)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Vid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("VID");
            entity.Property(e => e.Vnr)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("VNR");
            entity.Property(e => e.Wrtze)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WRTZE");
            entity.Property(e => e.WrtzeEinheit)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WRTZE_Einheit");
        });

        modelBuilder.Entity<WorkArea>(entity =>
        {
            entity.HasKey(e => e.Bid).HasName("PK_tblbereich");

            entity.ToTable("WorkArea");

            entity.Property(e => e.Bid).HasColumnName("BID");
            entity.Property(e => e.Abteilung).HasMaxLength(255);
            entity.Property(e => e.Bereich).HasMaxLength(255);
            entity.Property(e => e.Sort).HasColumnName("SORT");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
