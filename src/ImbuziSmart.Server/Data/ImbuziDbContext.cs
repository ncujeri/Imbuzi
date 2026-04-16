using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImbuziSmart.Server.Identity;
using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Server.Data;

public class ImbuziDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    private readonly Guid _tenantId;

    // ── Livestock tables ───────────────────────────────────────────────────────
    public DbSet<Animal>       Animals       => Set<Animal>();
    public DbSet<WeightRecord> WeightRecords => Set<WeightRecord>();
    public DbSet<MatingRecord> MatingRecords => Set<MatingRecord>();
    public DbSet<HeatRecord>   HeatRecords   => Set<HeatRecord>();
    public DbSet<MedicalLog>   MedicalLogs   => Set<MedicalLog>();
    public DbSet<CostEntry>    CostEntries   => Set<CostEntry>();
    public DbSet<FarmSettings> FarmSettings  => Set<FarmSettings>();

    // ── Tenant registry ────────────────────────────────────────────────────────
    public DbSet<Tenant> Tenants => Set<Tenant>();

    public ImbuziDbContext(
        DbContextOptions<ImbuziDbContext> options,
        IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        var tenantClaim = httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id");
        _tenantId = tenantClaim is not null && Guid.TryParse(tenantClaim.Value, out var tid)
            ? tid
            : Guid.Empty;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // IMPORTANT: let IdentityDbContext configure Identity tables first
        base.OnModelCreating(modelBuilder);

        // ── Tenant ────────────────────────────────────────────────────────────
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).HasMaxLength(200).IsRequired();
            entity.Property(t => t.ContactEmail).HasMaxLength(256);
        });

        // ── AppUser — link to Tenant ──────────────────────────────────────────
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.HasOne(u => u.Tenant)
                  .WithMany()
                  .HasForeignKey(u => u.TenantId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(u => u.TenantId);
        });

        // ── Global tenant query filters ───────────────────────────────────────
        modelBuilder.Entity<Animal>().HasQueryFilter(e => e.TenantId == _tenantId);
        modelBuilder.Entity<WeightRecord>().HasQueryFilter(e => e.TenantId == _tenantId);
        modelBuilder.Entity<MatingRecord>().HasQueryFilter(e => e.TenantId == _tenantId);
        modelBuilder.Entity<HeatRecord>().HasQueryFilter(e => e.TenantId == _tenantId);
        modelBuilder.Entity<MedicalLog>().HasQueryFilter(e => e.TenantId == _tenantId);
        modelBuilder.Entity<CostEntry>().HasQueryFilter(e => e.TenantId == _tenantId);
        modelBuilder.Entity<FarmSettings>().HasQueryFilter(e => e.TenantId == _tenantId);

        // ── WeightRecord ──────────────────────────────────────────────────────
        modelBuilder.Entity<WeightRecord>(entity =>
        {
            entity.HasIndex(w => new { w.TenantId, w.AnimalId });
            entity.Property(w => w.Method).HasMaxLength(20).HasDefaultValue("Scale");
        });

        // ── Animal ────────────────────────────────────────────────────────────
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasIndex(a => new { a.TenantId, a.Tag }).IsUnique();
            entity.Property(a => a.Tag).HasMaxLength(50);
            entity.Property(a => a.Name).HasMaxLength(100);
            entity.Property(a => a.SalePrice).HasPrecision(18, 2);
            entity.Ignore(a => a.Photos);
        });

        // ── MatingRecord ──────────────────────────────────────────────────────
        modelBuilder.Entity<MatingRecord>(entity =>
        {
            entity.Ignore(m => m.ExpectedKiddingDate);
            entity.HasIndex(m => new { m.TenantId, m.DoeId, m.MatingDate });
        });

        // ── HeatRecord ────────────────────────────────────────────────────────
        modelBuilder.Entity<HeatRecord>(entity =>
        {
            entity.Ignore(h => h.NextExpectedHeat);
            entity.HasIndex(h => new { h.TenantId, h.AnimalId });
        });

        // ── MedicalLog ────────────────────────────────────────────────────────
        modelBuilder.Entity<MedicalLog>(entity =>
        {
            entity.Ignore(m => m.WithdrawalEndDate);
            entity.Property(m => m.Medication).HasMaxLength(200);
            entity.HasIndex(m => new { m.TenantId, m.AnimalId });
        });

        // ── CostEntry ─────────────────────────────────────────────────────────
        modelBuilder.Entity<CostEntry>(entity =>
        {
            entity.Property(c => c.Amount).HasPrecision(18, 2);
            entity.HasIndex(c => new { c.TenantId, c.AnimalId });
        });

        // ── FarmSettings ──────────────────────────────────────────────────────
        modelBuilder.Entity<FarmSettings>(entity =>
        {
            entity.HasIndex(f => f.TenantId).IsUnique();
            entity.Property(f => f.Currency).HasMaxLength(3).HasDefaultValue("ZAR");
        });

        // ── Seed demo data ────────────────────────────────────────────────────
        modelBuilder.Entity<Animal>().HasData(SeedData.GetAnimals());
        modelBuilder.Entity<MatingRecord>().HasData(SeedData.GetMatingRecords());
        modelBuilder.Entity<HeatRecord>().HasData(SeedData.GetHeatRecords());
        modelBuilder.Entity<MedicalLog>().HasData(SeedData.GetMedicalLogs());
        modelBuilder.Entity<CostEntry>().HasData(SeedData.GetCostEntries());
        modelBuilder.Entity<FarmSettings>().HasData(SeedData.GetFarmSettings());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                if (entry.Entity.TenantId == Guid.Empty)
                    entry.Entity.TenantId = _tenantId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
