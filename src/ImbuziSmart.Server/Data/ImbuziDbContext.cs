using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ImbuziSmart.Server.Identity;
using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Server.Data;

public class ImbuziDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    private readonly Guid   _tenantId;
    private readonly string _currentUser;

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

    // ── Notifications ──────────────────────────────────────────────────────────
    public DbSet<AppNotification> Notifications => Set<AppNotification>();

    public ImbuziDbContext(
        DbContextOptions<ImbuziDbContext> options,
        IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        var user      = httpContextAccessor.HttpContext?.User;
        var tenantClaim = user?.FindFirst("tenant_id");
        _tenantId = tenantClaim is not null && Guid.TryParse(tenantClaim.Value, out var tid)
            ? tid
            : Guid.Empty;

        // Full name from JWT — falls back to email, then "System"
        _currentUser = user?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
            ?? user?.FindFirst("email")?.Value
            ?? "System";
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
        modelBuilder.Entity<AppNotification>().HasQueryFilter(n => n.TenantId == _tenantId);

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

        // ── AppNotification ────────────────────────────────────────────────────
        modelBuilder.Entity<AppNotification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Title).HasMaxLength(200).IsRequired();
            entity.Property(n => n.Message).HasMaxLength(1000);
            entity.Property(n => n.EntityType).HasMaxLength(50);
            entity.Property(n => n.CreatedBy).HasMaxLength(256);
            entity.HasIndex(n => new { n.TenantId, n.IsRead, n.CreatedAt });
        });

        // Demo farm data is seeded at startup by DatabaseSeeder (admin tenant only).
        // No HasData here — seeding is tenant-scoped to admin@imbuzi.app.
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt  = DateTime.UtcNow;
                entry.Entity.CreatedBy  = _currentUser;
                if (entry.Entity.TenantId == Guid.Empty)
                    entry.Entity.TenantId = _tenantId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt        = DateTime.UtcNow;
                entry.Entity.LastModifiedBy   = _currentUser;
                // Never let a client push overwrite the original creator
                entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
