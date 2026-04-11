using Microsoft.EntityFrameworkCore;
using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Server.Data;

public class ImbuziDbContext : DbContext
{
    private readonly Guid _tenantId;

    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<MatingRecord> MatingRecords => Set<MatingRecord>();
    public DbSet<HeatRecord> HeatRecords => Set<HeatRecord>();
    public DbSet<MedicalLog> MedicalLogs => Set<MedicalLog>();
    public DbSet<CostEntry> CostEntries => Set<CostEntry>();
    public DbSet<FarmSettings> FarmSettings => Set<FarmSettings>();

    public ImbuziDbContext(DbContextOptions<ImbuziDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        var tenantClaim = httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id");
        _tenantId = tenantClaim is not null && Guid.TryParse(tenantClaim.Value, out var tid)
            ? tid
            : Guid.Empty;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Global tenant query filters — every query is automatically scoped
        modelBuilder.Entity<Animal>().HasQueryFilter(e => e.TenantId == _tenantId);
        modelBuilder.Entity<MatingRecord>().HasQueryFilter(e => e.TenantId == _tenantId);
        modelBuilder.Entity<HeatRecord>().HasQueryFilter(e => e.TenantId == _tenantId);
        modelBuilder.Entity<MedicalLog>().HasQueryFilter(e => e.TenantId == _tenantId);
        modelBuilder.Entity<CostEntry>().HasQueryFilter(e => e.TenantId == _tenantId);
        modelBuilder.Entity<FarmSettings>().HasQueryFilter(e => e.TenantId == _tenantId);

        // Animal — self-referencing parentage
        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasIndex(a => new { a.TenantId, a.Tag }).IsUnique();
            entity.Property(a => a.Tag).HasMaxLength(50);
            entity.Property(a => a.Name).HasMaxLength(100);

            // Computed properties — not mapped
            entity.Ignore(a => a.Photos);
        });

        // MatingRecord — computed property not mapped
        modelBuilder.Entity<MatingRecord>(entity =>
        {
            entity.Ignore(m => m.ExpectedKiddingDate);
            entity.HasIndex(m => new { m.TenantId, m.DoeId, m.MatingDate });
        });

        // HeatRecord — computed property not mapped
        modelBuilder.Entity<HeatRecord>(entity =>
        {
            entity.Ignore(h => h.NextExpectedHeat);
            entity.HasIndex(h => new { h.TenantId, h.AnimalId });
        });

        // MedicalLog — computed property not mapped
        modelBuilder.Entity<MedicalLog>(entity =>
        {
            entity.Ignore(m => m.WithdrawalEndDate);
            entity.Property(m => m.Medication).HasMaxLength(200);
            entity.HasIndex(m => new { m.TenantId, m.AnimalId });
        });

        // CostEntry
        modelBuilder.Entity<CostEntry>(entity =>
        {
            entity.Property(c => c.Amount).HasPrecision(18, 2);
            entity.HasIndex(c => new { c.TenantId, c.AnimalId });
        });

        // FarmSettings — one per tenant
        modelBuilder.Entity<FarmSettings>(entity =>
        {
            entity.HasIndex(f => f.TenantId).IsUnique();
            entity.Property(f => f.Currency).HasMaxLength(3).HasDefaultValue("ZAR");
        });
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
