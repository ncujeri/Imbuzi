using ImbuziSmart.Shared.Entities;
using ImbuziSmart.Shared.Enums;

namespace ImbuziSmart.Server.Data;

/// <summary>
/// Provides static seed data for EF Core migrations.
/// Mirrors the client-side SeedDataService for consistency.
/// </summary>
public static class SeedData
{
    // Demo tenant
    public static readonly Guid DemoTenantId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

    // Animal IDs
    private static readonly Guid BuckThembaId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private static readonly Guid BuckMandlaId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    private static readonly Guid DoeNandiId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    private static readonly Guid DoeLindiweId = Guid.Parse("20000000-0000-0000-0000-000000000002");
    private static readonly Guid DoeNomalusaId = Guid.Parse("20000000-0000-0000-0000-000000000003");
    private static readonly Guid DoeThembiId = Guid.Parse("20000000-0000-0000-0000-000000000004");
    private static readonly Guid DoeZaneleId = Guid.Parse("20000000-0000-0000-0000-000000000005");
    private static readonly Guid KidSiphoId = Guid.Parse("30000000-0000-0000-0000-000000000001");
    private static readonly Guid KidAyandaId = Guid.Parse("30000000-0000-0000-0000-000000000002");
    private static readonly Guid KidBusiId = Guid.Parse("30000000-0000-0000-0000-000000000003");
    private static readonly Guid KidLungileId = Guid.Parse("30000000-0000-0000-0000-000000000004");
    private static readonly Guid SoldDoeLesediId = Guid.Parse("40000000-0000-0000-0000-000000000001");

    // Use a fixed "today" for deterministic seed data in migrations
    private static readonly DateTime SeedDate = new(2026, 4, 11, 0, 0, 0, DateTimeKind.Utc);

    public static Animal[] GetAnimals() => new[]
    {
        new Animal
        {
            Id = BuckThembaId, TenantId = DemoTenantId,
            Name = "Themba", Tag = "BK-001", Species = Species.Goat, Breed = "Boer",
            Sex = Sex.Male, DateOfBirth = SeedDate.AddYears(-3).AddMonths(-2),
            Status = AnimalStatus.Active,
            Notes = "Dominant buck. Excellent build, strong Boer genetics.",
            CreatedAt = SeedDate.AddMonths(-6)
        },
        new Animal
        {
            Id = BuckMandlaId, TenantId = DemoTenantId,
            Name = "Mandla", Tag = "BK-002", Species = Species.Goat, Breed = "Boer Cross",
            Sex = Sex.Male, DateOfBirth = SeedDate.AddYears(-2).AddMonths(-5),
            Status = AnimalStatus.Active,
            Notes = "Young buck, good temperament. Ready for first breeding season.",
            CreatedAt = SeedDate.AddMonths(-4)
        },
        new Animal
        {
            Id = DoeNandiId, TenantId = DemoTenantId,
            Name = "Nandi", Tag = "DE-001", Species = Species.Goat, Breed = "Boer",
            Sex = Sex.Female, DateOfBirth = SeedDate.AddYears(-4).AddMonths(-1),
            Status = AnimalStatus.Active,
            Notes = "Top producer — 3 successful kiddings. Always twins.",
            CreatedAt = SeedDate.AddMonths(-8)
        },
        new Animal
        {
            Id = DoeLindiweId, TenantId = DemoTenantId,
            Name = "Lindiwe", Tag = "DE-002", Species = Species.Goat, Breed = "Boer",
            Sex = Sex.Female, DateOfBirth = SeedDate.AddYears(-3).AddMonths(-6),
            SireId = BuckThembaId, Status = AnimalStatus.Active,
            Notes = "Daughter of Themba. Currently pregnant — due soon.",
            CreatedAt = SeedDate.AddMonths(-7)
        },
        new Animal
        {
            Id = DoeNomalusaId, TenantId = DemoTenantId,
            Name = "Nomalusa", Tag = "DE-003", Species = Species.Goat, Breed = "Indigenous Veld",
            Sex = Sex.Female, DateOfBirth = SeedDate.AddYears(-2).AddMonths(-8),
            Status = AnimalStatus.Active,
            Notes = "Hardy doe, good forager. First pregnancy.",
            CreatedAt = SeedDate.AddMonths(-5)
        },
        new Animal
        {
            Id = DoeThembiId, TenantId = DemoTenantId,
            Name = "Thembi", Tag = "DE-004", Species = Species.Goat, Breed = "Boer",
            Sex = Sex.Female, DateOfBirth = SeedDate.AddYears(-3),
            DamId = DoeNandiId, Status = AnimalStatus.Active,
            Notes = "Daughter of Nandi. Good maternal instincts.",
            CreatedAt = SeedDate.AddMonths(-6)
        },
        new Animal
        {
            Id = DoeZaneleId, TenantId = DemoTenantId,
            Name = "Zanele", Tag = "DE-005", Species = Species.Goat, Breed = "Savanna",
            Sex = Sex.Female, DateOfBirth = SeedDate.AddYears(-2).AddMonths(-3),
            Status = AnimalStatus.Active,
            Notes = "White Savanna doe, heat observed recently.",
            CreatedAt = SeedDate.AddMonths(-3)
        },
        new Animal
        {
            Id = KidSiphoId, TenantId = DemoTenantId,
            Name = "Sipho", Tag = "KD-001", Species = Species.Goat, Breed = "Boer",
            Sex = Sex.Male, DateOfBirth = SeedDate.AddDays(-75),
            SireId = BuckThembaId, DamId = DoeNandiId, Status = AnimalStatus.Active,
            Notes = "Strong kid, growing well. Weaning approaching.",
            CreatedAt = SeedDate.AddDays(-75)
        },
        new Animal
        {
            Id = KidAyandaId, TenantId = DemoTenantId,
            Name = "Ayanda", Tag = "KD-002", Species = Species.Goat, Breed = "Boer",
            Sex = Sex.Female, DateOfBirth = SeedDate.AddDays(-75),
            SireId = BuckThembaId, DamId = DoeNandiId, Status = AnimalStatus.Active,
            Notes = "Twin of Sipho. Slightly smaller but healthy.",
            CreatedAt = SeedDate.AddDays(-75)
        },
        new Animal
        {
            Id = KidBusiId, TenantId = DemoTenantId,
            Name = "Busi", Tag = "KD-003", Species = Species.Goat, Breed = "Boer Cross",
            Sex = Sex.Female, DateOfBirth = SeedDate.AddDays(-100),
            SireId = BuckMandlaId, DamId = DoeThembiId, Status = AnimalStatus.Active,
            Notes = "Weaned successfully. Ready for next phase.",
            CreatedAt = SeedDate.AddDays(-100)
        },
        new Animal
        {
            Id = KidLungileId, TenantId = DemoTenantId,
            Name = "Lungile", Tag = "KD-004", Species = Species.Goat, Breed = "Indigenous Veld",
            Sex = Sex.Male, DateOfBirth = SeedDate.AddMonths(-6),
            DamId = DoeNomalusaId, Status = AnimalStatus.MarketReady,
            Notes = "Good weight, cleared withdrawal. Ready for market.",
            CreatedAt = SeedDate.AddMonths(-6)
        },
        new Animal
        {
            Id = SoldDoeLesediId, TenantId = DemoTenantId,
            Name = "Lesedi", Tag = "DE-006", Species = Species.Goat, Breed = "Boer",
            Sex = Sex.Female, DateOfBirth = SeedDate.AddYears(-2),
            Status = AnimalStatus.Sold,
            Notes = "Sold at Vryburg auction — R3,200.",
            CreatedAt = SeedDate.AddMonths(-10)
        }
    };

    public static MatingRecord[] GetMatingRecords() => new[]
    {
        new MatingRecord
        {
            Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
            TenantId = DemoTenantId,
            BuckId = BuckThembaId, DoeId = DoeNandiId,
            MatingDate = SeedDate.AddDays(-225),
            ActualKiddingDate = SeedDate.AddDays(-75),
            Notes = "Twins born — both healthy. Sipho (male) and Ayanda (female).",
            CreatedAt = SeedDate.AddDays(-225)
        },
        new MatingRecord
        {
            Id = Guid.Parse("50000000-0000-0000-0000-000000000002"),
            TenantId = DemoTenantId,
            BuckId = BuckMandlaId, DoeId = DoeLindiweId,
            MatingDate = SeedDate.AddDays(-147),
            Notes = "First mating for Mandla. Lindiwe showing signs of nearing kidding.",
            CreatedAt = SeedDate.AddDays(-147)
        },
        new MatingRecord
        {
            Id = Guid.Parse("50000000-0000-0000-0000-000000000003"),
            TenantId = DemoTenantId,
            BuckId = BuckThembaId, DoeId = DoeNomalusaId,
            MatingDate = SeedDate.AddDays(-120),
            Notes = "First pregnancy for Nomalusa. Progressing well.",
            CreatedAt = SeedDate.AddDays(-120)
        },
        new MatingRecord
        {
            Id = Guid.Parse("50000000-0000-0000-0000-000000000004"),
            TenantId = DemoTenantId,
            BuckId = BuckMandlaId, DoeId = DoeThembiId,
            MatingDate = SeedDate.AddDays(-250),
            ActualKiddingDate = SeedDate.AddDays(-100),
            Notes = "Single kid born — Busi (female). Easy delivery.",
            CreatedAt = SeedDate.AddDays(-250)
        }
    };

    public static HeatRecord[] GetHeatRecords() => new[]
    {
        new HeatRecord
        {
            Id = Guid.Parse("60000000-0000-0000-0000-000000000001"),
            TenantId = DemoTenantId,
            AnimalId = DoeZaneleId, ObservedDate = SeedDate.AddDays(-19),
            Signs = "Flagging tail, frequent vocalization, mounting other does",
            Intensity = HeatIntensity.Strong,
            CreatedAt = SeedDate.AddDays(-19)
        },
        new HeatRecord
        {
            Id = Guid.Parse("60000000-0000-0000-0000-000000000002"),
            TenantId = DemoTenantId,
            AnimalId = DoeZaneleId, ObservedDate = SeedDate.AddDays(-40),
            Signs = "Mild flagging, slight swelling",
            Intensity = HeatIntensity.Mild,
            CreatedAt = SeedDate.AddDays(-40)
        },
        new HeatRecord
        {
            Id = Guid.Parse("60000000-0000-0000-0000-000000000003"),
            TenantId = DemoTenantId,
            AnimalId = DoeThembiId, ObservedDate = SeedDate.AddDays(-10),
            Signs = "Standing heat, accepted buck",
            Intensity = HeatIntensity.Standing,
            CreatedAt = SeedDate.AddDays(-10)
        },
        new HeatRecord
        {
            Id = Guid.Parse("60000000-0000-0000-0000-000000000004"),
            TenantId = DemoTenantId,
            AnimalId = DoeNandiId, ObservedDate = SeedDate.AddDays(-5),
            Signs = "Restless, vocalizing, swollen vulva",
            Intensity = HeatIntensity.Strong,
            CreatedAt = SeedDate.AddDays(-5)
        }
    };

    public static MedicalLog[] GetMedicalLogs() => new[]
    {
        new MedicalLog
        {
            Id = Guid.Parse("70000000-0000-0000-0000-000000000001"),
            TenantId = DemoTenantId,
            AnimalId = BuckThembaId, TreatmentDate = SeedDate.AddDays(-45),
            Medication = "Ivermectin", Dosage = "1ml per 10kg body weight",
            WithdrawalPeriodDays = 35, VetName = "Dr. Mokoena",
            Notes = "Routine quarterly deworming. Withdrawal period expired.",
            CreatedAt = SeedDate.AddDays(-45)
        },
        new MedicalLog
        {
            Id = Guid.Parse("70000000-0000-0000-0000-000000000002"),
            TenantId = DemoTenantId,
            AnimalId = KidSiphoId, TreatmentDate = SeedDate.AddDays(-9),
            Medication = "Sulfadimidine", Dosage = "1ml per 5kg",
            WithdrawalPeriodDays = 14, VetName = "Dr. Mokoena",
            Notes = "Mild scours, responding well to treatment.",
            CreatedAt = SeedDate.AddDays(-9)
        },
        new MedicalLog
        {
            Id = Guid.Parse("70000000-0000-0000-0000-000000000003"),
            TenantId = DemoTenantId,
            AnimalId = DoeNandiId, TreatmentDate = SeedDate.AddDays(-30),
            Medication = "Pulpy Kidney Vaccine", Dosage = "2ml subcutaneous",
            Notes = "Annual Clostridial vaccination. No withdrawal required.",
            CreatedAt = SeedDate.AddDays(-30)
        },
        new MedicalLog
        {
            Id = Guid.Parse("70000000-0000-0000-0000-000000000004"),
            TenantId = DemoTenantId,
            AnimalId = BuckMandlaId, TreatmentDate = SeedDate.AddDays(-6),
            Medication = "Oxytetracycline LA", Dosage = "1ml per 10kg IM",
            WithdrawalPeriodDays = 14, VetName = "Dr. Mokoena",
            Notes = "Mild foot rot on front left. Hoof trimmed and treated.",
            CreatedAt = SeedDate.AddDays(-6)
        },
        new MedicalLog
        {
            Id = Guid.Parse("70000000-0000-0000-0000-000000000005"),
            TenantId = DemoTenantId,
            AnimalId = KidLungileId, TreatmentDate = SeedDate.AddDays(-60),
            Medication = "Albendazole", Dosage = "5ml oral",
            WithdrawalPeriodDays = 14,
            Notes = "Pre-market deworming. Withdrawal period cleared.",
            CreatedAt = SeedDate.AddDays(-60)
        },
        new MedicalLog
        {
            Id = Guid.Parse("70000000-0000-0000-0000-000000000006"),
            TenantId = DemoTenantId,
            AnimalId = DoeLindiweId, TreatmentDate = SeedDate.AddDays(-20),
            Medication = "Vitamin ADE Injection", Dosage = "2ml IM",
            Notes = "Pre-kidding vitamin boost. No withdrawal.",
            CreatedAt = SeedDate.AddDays(-20)
        }
    };

    public static CostEntry[] GetCostEntries() => new[]
    {
        new CostEntry
        {
            Id = Guid.Parse("80000000-0000-0000-0000-000000000001"),
            TenantId = DemoTenantId,
            Category = CostCategory.Feed, Amount = 1200.00m,
            Date = SeedDate.AddDays(-30),
            Description = "Lucerne bales x 20 — monthly feed supply",
            CreatedAt = SeedDate.AddDays(-30)
        },
        new CostEntry
        {
            Id = Guid.Parse("80000000-0000-0000-0000-000000000002"),
            TenantId = DemoTenantId,
            Category = CostCategory.Feed, Amount = 450.00m,
            Date = SeedDate.AddDays(-15),
            Description = "Maize meal 50kg bag — supplementary feeding for pregnant does",
            CreatedAt = SeedDate.AddDays(-15)
        },
        new CostEntry
        {
            Id = Guid.Parse("80000000-0000-0000-0000-000000000003"),
            TenantId = DemoTenantId,
            Category = CostCategory.Feed, Amount = 280.00m,
            Date = SeedDate.AddDays(-7),
            Description = "Mineral lick blocks x 4",
            CreatedAt = SeedDate.AddDays(-7)
        },
        new CostEntry
        {
            Id = Guid.Parse("80000000-0000-0000-0000-000000000004"),
            TenantId = DemoTenantId,
            AnimalId = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            Category = CostCategory.Veterinary, Amount = 350.00m,
            Date = SeedDate.AddDays(-6),
            Description = "Dr. Mokoena — foot rot treatment and hoof trim",
            CreatedAt = SeedDate.AddDays(-6)
        },
        new CostEntry
        {
            Id = Guid.Parse("80000000-0000-0000-0000-000000000005"),
            TenantId = DemoTenantId,
            AnimalId = Guid.Parse("30000000-0000-0000-0000-000000000001"),
            Category = CostCategory.Veterinary, Amount = 180.00m,
            Date = SeedDate.AddDays(-9),
            Description = "Dr. Mokoena — scours treatment for Sipho",
            CreatedAt = SeedDate.AddDays(-9)
        },
        new CostEntry
        {
            Id = Guid.Parse("80000000-0000-0000-0000-000000000006"),
            TenantId = DemoTenantId,
            Category = CostCategory.Medication, Amount = 520.00m,
            Date = SeedDate.AddDays(-45),
            Description = "Ivermectin 500ml, Pulpy Kidney Vaccine x 20 doses — quarterly stock",
            CreatedAt = SeedDate.AddDays(-45)
        },
        new CostEntry
        {
            Id = Guid.Parse("80000000-0000-0000-0000-000000000007"),
            TenantId = DemoTenantId,
            Category = CostCategory.Labor, Amount = 2000.00m,
            Date = SeedDate.AddDays(-1),
            Description = "Sbusiso — monthly herding and general farm labor",
            CreatedAt = SeedDate.AddDays(-1)
        },
        new CostEntry
        {
            Id = Guid.Parse("80000000-0000-0000-0000-000000000008"),
            TenantId = DemoTenantId,
            Category = CostCategory.Labor, Amount = 2000.00m,
            Date = SeedDate.AddMonths(-1),
            Description = "Sbusiso — monthly herding and general farm labor",
            CreatedAt = SeedDate.AddMonths(-1)
        },
        new CostEntry
        {
            Id = Guid.Parse("80000000-0000-0000-0000-000000000009"),
            TenantId = DemoTenantId,
            Category = CostCategory.Equipment, Amount = 850.00m,
            Date = SeedDate.AddDays(-20),
            Description = "Ear tag applicator + 50 tags",
            CreatedAt = SeedDate.AddDays(-20)
        },
        new CostEntry
        {
            Id = Guid.Parse("80000000-0000-0000-0000-000000000010"),
            TenantId = DemoTenantId,
            AnimalId = Guid.Parse("40000000-0000-0000-0000-000000000001"),
            Category = CostCategory.Feed, Amount = 600.00m,
            Date = SeedDate.AddMonths(-3),
            Description = "Fattening feed for Lesedi before auction",
            CreatedAt = SeedDate.AddMonths(-3)
        }
    };

    public static FarmSettings[] GetFarmSettings() => new[]
    {
        new FarmSettings
        {
            Id = Guid.Parse("90000000-0000-0000-0000-000000000001"),
            TenantId = DemoTenantId,
            EnableInbreedingCheck = true,
            EnableHeatTracking = true,
            HeatCycleDays = 21,
            EnablePushNotifications = false,
            NotifyKiddingWatch = true,
            NotifyWeaning = true,
            NotifyWithdrawalExpiry = true,
            NotifyHeatCycle = true,
            KiddingWatchDaysBefore = 5,
            HeatAlertDaysBefore = 2,
            Currency = "ZAR",
            CreatedAt = SeedDate
        }
    };
}
