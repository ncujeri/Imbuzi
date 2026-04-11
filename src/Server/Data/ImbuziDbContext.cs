using ImbuziSmart.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImbuziSmart.Server.Data;

public class ImbuziDbContext : DbContext
{
    public ImbuziDbContext(DbContextOptions<ImbuziDbContext> options) : base(options) { }

    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<MatingRecord> MatingRecords => Set<MatingRecord>();
    public DbSet<HeatRecord> HeatRecords => Set<HeatRecord>();
    public DbSet<MedicalLog> MedicalLogs => Set<MedicalLog>();
    public DbSet<CostEntry> CostEntries => Set<CostEntry>();
    public DbSet<FarmSettings> FarmSettings => Set<FarmSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Global tenant query filters for multi-tenancy
        // Uncomment when tenant resolution is wired up:
        // modelBuilder.Entity<Animal>().HasQueryFilter(a => a.TenantId == _currentTenantId);
        // modelBuilder.Entity<MatingRecord>().HasQueryFilter(m => m.TenantId == _currentTenantId);
        // etc.

        modelBuilder.Entity<Animal>(entity =>
        {
            entity.HasIndex(a => new { a.TenantId, a.Tag }).IsUnique();
            entity.Property(a => a.Name).HasMaxLength(100);
            entity.Property(a => a.Tag).HasMaxLength(50);
            entity.Property(a => a.Breed).HasMaxLength(100);
            entity.Property(a => a.Notes).HasMaxLength(2000);

            // Self-referencing parentage
            entity.HasOne<Animal>()
                .WithMany()
                .HasForeignKey(a => a.SireId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Animal>()
                .WithMany()
                .HasForeignKey(a => a.DamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Photos and WeightHistory stored as JSON
            entity.OwnsMany(a => a.Photos, p => p.ToJson());
            entity.OwnsMany(a => a.WeightHistory, w => w.ToJson());
        });

        modelBuilder.Entity<MatingRecord>(entity =>
        {
            entity.Ignore(m => m.ExpectedKiddingDate);
            entity.Property(m => m.Notes).HasMaxLength(2000);
        });

        modelBuilder.Entity<HeatRecord>(entity =>
        {
            entity.Ignore(h => h.NextExpectedHeat);
            entity.Property(h => h.Signs).HasMaxLength(500);
        });

        modelBuilder.Entity<MedicalLog>(entity =>
        {
            entity.Ignore(m => m.WithdrawalEndDate);
            entity.Property(m => m.Medication).HasMaxLength(200);
            entity.Property(m => m.Dosage).HasMaxLength(100);
            entity.Property(m => m.VetName).HasMaxLength(200);
            entity.Property(m => m.Notes).HasMaxLength(2000);
        });

        modelBuilder.Entity<CostEntry>(entity =>
        {
            entity.Property(c => c.Amount).HasPrecision(18, 2);
            entity.Property(c => c.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<FarmSettings>(entity =>
        {
            entity.HasIndex(f => f.TenantId).IsUnique();
            entity.Property(f => f.Currency).HasMaxLength(10).HasDefaultValue("ZAR");
        });
    }
}
