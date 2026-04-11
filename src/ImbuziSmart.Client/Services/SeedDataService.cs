using ImbuziSmart.Shared.Entities;
using ImbuziSmart.Shared.Enums;

namespace ImbuziSmart.Client.Services;

/// <summary>
/// Seeds IndexedDB with realistic demo data for a South African goat farm.
/// Only runs on first launch (checks for existing data).
/// </summary>
public class SeedDataService
{
    private readonly IndexedDbService _db;

    // Fixed demo tenant
    public static readonly Guid DemoTenantId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

    // Fixed animal IDs for referential integrity
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

    public SeedDataService(IndexedDbService db)
    {
        _db = db;
    }

    public async Task SeedIfEmptyAsync()
    {
        var existing = await _db.GetAllAsync<Animal>("animals");
        if (existing.Count > 0)
            return; // Already has data

        await SeedAnimalsAsync();
        await SeedMatingRecordsAsync();
        await SeedHeatRecordsAsync();
        await SeedMedicalLogsAsync();
        await SeedCostEntriesAsync();
        await SeedFarmSettingsAsync();
    }

    private async Task SeedAnimalsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var animals = new List<Animal>
        {
            // === BUCKS (males) ===
            new()
            {
                Id = BuckThembaId,
                TenantId = DemoTenantId,
                Name = "Themba",
                Tag = "BK-001",
                Species = Species.Goat,
                Breed = "Boer",
                Sex = Sex.Male,
                DateOfBirth = today.AddYears(-3).AddMonths(-2),
                Status = AnimalStatus.Active,
                Notes = "Dominant buck. Excellent build, strong Boer genetics.",
                CreatedAt = today.AddMonths(-6)
            },
            new()
            {
                Id = BuckMandlaId,
                TenantId = DemoTenantId,
                Name = "Mandla",
                Tag = "BK-002",
                Species = Species.Goat,
                Breed = "Boer Cross",
                Sex = Sex.Male,
                DateOfBirth = today.AddYears(-2).AddMonths(-5),
                Status = AnimalStatus.Active,
                Notes = "Young buck, good temperament. Ready for first breeding season.",
                CreatedAt = today.AddMonths(-4)
            },

            // === DOES (females) ===
            new()
            {
                Id = DoeNandiId,
                TenantId = DemoTenantId,
                Name = "Nandi",
                Tag = "DE-001",
                Species = Species.Goat,
                Breed = "Boer",
                Sex = Sex.Female,
                DateOfBirth = today.AddYears(-4).AddMonths(-1),
                Status = AnimalStatus.Active,
                Notes = "Top producer — 3 successful kiddings. Always twins.",
                CreatedAt = today.AddMonths(-8)
            },
            new()
            {
                Id = DoeLindiweId,
                TenantId = DemoTenantId,
                Name = "Lindiwe",
                Tag = "DE-002",
                Species = Species.Goat,
                Breed = "Boer",
                Sex = Sex.Female,
                DateOfBirth = today.AddYears(-3).AddMonths(-6),
                SireId = BuckThembaId,
                Status = AnimalStatus.Active,
                Notes = "Daughter of Themba. Currently pregnant — due soon.",
                CreatedAt = today.AddMonths(-7)
            },
            new()
            {
                Id = DoeNomalusaId,
                TenantId = DemoTenantId,
                Name = "Nomalusa",
                Tag = "DE-003",
                Species = Species.Goat,
                Breed = "Indigenous Veld",
                Sex = Sex.Female,
                DateOfBirth = today.AddYears(-2).AddMonths(-8),
                Status = AnimalStatus.Active,
                Notes = "Hardy doe, good forager. First pregnancy.",
                CreatedAt = today.AddMonths(-5)
            },
            new()
            {
                Id = DoeThembiId,
                TenantId = DemoTenantId,
                Name = "Thembi",
                Tag = "DE-004",
                Species = Species.Goat,
                Breed = "Boer",
                Sex = Sex.Female,
                DateOfBirth = today.AddYears(-3),
                DamId = DoeNandiId,
                Status = AnimalStatus.Active,
                Notes = "Daughter of Nandi. Good maternal instincts.",
                CreatedAt = today.AddMonths(-6)
            },
            new()
            {
                Id = DoeZaneleId,
                TenantId = DemoTenantId,
                Name = "Zanele",
                Tag = "DE-005",
                Species = Species.Goat,
                Breed = "Savanna",
                Sex = Sex.Female,
                DateOfBirth = today.AddYears(-2).AddMonths(-3),
                Status = AnimalStatus.Active,
                Notes = "White Savanna doe, heat observed recently.",
                CreatedAt = today.AddMonths(-3)
            },

            // === KIDS (young) ===
            new()
            {
                Id = KidSiphoId,
                TenantId = DemoTenantId,
                Name = "Sipho",
                Tag = "KD-001",
                Species = Species.Goat,
                Breed = "Boer",
                Sex = Sex.Male,
                DateOfBirth = today.AddDays(-75),
                SireId = BuckThembaId,
                DamId = DoeNandiId,
                Status = AnimalStatus.Active,
                Notes = "Strong kid, growing well. Weaning approaching.",
                CreatedAt = today.AddDays(-75)
            },
            new()
            {
                Id = KidAyandaId,
                TenantId = DemoTenantId,
                Name = "Ayanda",
                Tag = "KD-002",
                Species = Species.Goat,
                Breed = "Boer",
                Sex = Sex.Female,
                DateOfBirth = today.AddDays(-75),
                SireId = BuckThembaId,
                DamId = DoeNandiId,
                Status = AnimalStatus.Active,
                Notes = "Twin of Sipho. Slightly smaller but healthy.",
                CreatedAt = today.AddDays(-75)
            },
            new()
            {
                Id = KidBusiId,
                TenantId = DemoTenantId,
                Name = "Busi",
                Tag = "KD-003",
                Species = Species.Goat,
                Breed = "Boer Cross",
                Sex = Sex.Female,
                DateOfBirth = today.AddDays(-100),
                SireId = BuckMandlaId,
                DamId = DoeThembiId,
                Status = AnimalStatus.Active,
                Notes = "Weaned successfully. Ready for next phase.",
                CreatedAt = today.AddDays(-100)
            },
            new()
            {
                Id = KidLungileId,
                TenantId = DemoTenantId,
                Name = "Lungile",
                Tag = "KD-004",
                Species = Species.Goat,
                Breed = "Indigenous Veld",
                Sex = Sex.Male,
                DateOfBirth = today.AddMonths(-6),
                DamId = DoeNomalusaId,
                Status = AnimalStatus.MarketReady,
                Notes = "Good weight, cleared withdrawal. Ready for market.",
                CreatedAt = today.AddMonths(-6)
            },

            // === SOLD ===
            new()
            {
                Id = SoldDoeLesediId,
                TenantId = DemoTenantId,
                Name = "Lesedi",
                Tag = "DE-006",
                Species = Species.Goat,
                Breed = "Boer",
                Sex = Sex.Female,
                DateOfBirth = today.AddYears(-2),
                Status = AnimalStatus.Sold,
                Notes = "Sold at Vryburg auction — R3,200.",
                CreatedAt = today.AddMonths(-10)
            }
        };

        foreach (var animal in animals)
        {
            await _db.PutAsync("animals", animal);
        }
    }

    private async Task SeedMatingRecordsAsync()
    {
        var today = DateTime.UtcNow.Date;

        var matingRecords = new List<MatingRecord>
        {
            // Nandi x Themba — kidded 75 days ago (twins: Sipho + Ayanda)
            new()
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                TenantId = DemoTenantId,
                BuckId = BuckThembaId,
                DoeId = DoeNandiId,
                MatingDate = today.AddDays(-75).AddDays(-150),
                ActualKiddingDate = today.AddDays(-75),
                Notes = "Twins born — both healthy. Sipho (male) and Ayanda (female).",
                CreatedAt = today.AddDays(-225)
            },

            // Lindiwe x Mandla — currently pregnant, 147 days in (KIDDING WATCH!)
            new()
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000002"),
                TenantId = DemoTenantId,
                BuckId = BuckMandlaId,
                DoeId = DoeLindiweId,
                MatingDate = today.AddDays(-147),
                Notes = "First mating for Mandla. Lindiwe showing signs of nearing kidding.",
                CreatedAt = today.AddDays(-147)
            },

            // Nomalusa x Themba — currently pregnant, 120 days in
            new()
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000003"),
                TenantId = DemoTenantId,
                BuckId = BuckThembaId,
                DoeId = DoeNomalusaId,
                MatingDate = today.AddDays(-120),
                Notes = "First pregnancy for Nomalusa. Progressing well.",
                CreatedAt = today.AddDays(-120)
            },

            // Thembi x Mandla — kidded 100 days ago (Busi)
            new()
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000004"),
                TenantId = DemoTenantId,
                BuckId = BuckMandlaId,
                DoeId = DoeThembiId,
                MatingDate = today.AddDays(-100).AddDays(-150),
                ActualKiddingDate = today.AddDays(-100),
                Notes = "Single kid born — Busi (female). Easy delivery.",
                CreatedAt = today.AddDays(-250)
            }
        };

        foreach (var record in matingRecords)
        {
            await _db.PutAsync("matingRecords", record);
        }
    }

    private async Task SeedHeatRecordsAsync()
    {
        var today = DateTime.UtcNow.Date;

        var heatRecords = new List<HeatRecord>
        {
            // Zanele — observed in heat 19 days ago (next heat expected in ~2 days!)
            new()
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                TenantId = DemoTenantId,
                AnimalId = DoeZaneleId,
                ObservedDate = today.AddDays(-19),
                Signs = "Flagging tail, frequent vocalization, mounting other does",
                Intensity = HeatIntensity.Strong,
                CreatedAt = today.AddDays(-19)
            },
            // Zanele — previous cycle
            new()
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000002"),
                TenantId = DemoTenantId,
                AnimalId = DoeZaneleId,
                ObservedDate = today.AddDays(-40),
                Signs = "Mild flagging, slight swelling",
                Intensity = HeatIntensity.Mild,
                CreatedAt = today.AddDays(-40)
            },
            // Thembi — observed 10 days ago
            new()
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000003"),
                TenantId = DemoTenantId,
                AnimalId = DoeThembiId,
                ObservedDate = today.AddDays(-10),
                Signs = "Standing heat, accepted buck",
                Intensity = HeatIntensity.Standing,
                CreatedAt = today.AddDays(-10)
            },
            // Nandi — 5 days ago (recently after weaning window)
            new()
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000004"),
                TenantId = DemoTenantId,
                AnimalId = DoeNandiId,
                ObservedDate = today.AddDays(-5),
                Signs = "Restless, vocalizing, swollen vulva",
                Intensity = HeatIntensity.Strong,
                CreatedAt = today.AddDays(-5)
            }
        };

        foreach (var record in heatRecords)
        {
            await _db.PutAsync("heatRecords", record);
        }
    }

    private async Task SeedMedicalLogsAsync()
    {
        var today = DateTime.UtcNow.Date;

        var medicalLogs = new List<MedicalLog>
        {
            // Themba — routine deworming (no withdrawal, completed)
            new()
            {
                Id = Guid.Parse("70000000-0000-0000-0000-000000000001"),
                TenantId = DemoTenantId,
                AnimalId = BuckThembaId,
                TreatmentDate = today.AddDays(-45),
                Medication = "Ivermectin",
                Dosage = "1ml per 10kg body weight",
                WithdrawalPeriodDays = 35,
                VetName = "Dr. Mokoena",
                Notes = "Routine quarterly deworming. Withdrawal period expired.",
                CreatedAt = today.AddDays(-45)
            },

            // Sipho (kid) — treated for scours, ACTIVE withdrawal (5 days remaining)
            new()
            {
                Id = Guid.Parse("70000000-0000-0000-0000-000000000002"),
                TenantId = DemoTenantId,
                AnimalId = KidSiphoId,
                TreatmentDate = today.AddDays(-9),
                Medication = "Sulfadimidine",
                Dosage = "1ml per 5kg",
                WithdrawalPeriodDays = 14,
                VetName = "Dr. Mokoena",
                Notes = "Mild scours, responding well to treatment.",
                CreatedAt = today.AddDays(-9)
            },

            // Nandi — vaccination (no withdrawal)
            new()
            {
                Id = Guid.Parse("70000000-0000-0000-0000-000000000003"),
                TenantId = DemoTenantId,
                AnimalId = DoeNandiId,
                TreatmentDate = today.AddDays(-30),
                Medication = "Pulpy Kidney Vaccine",
                Dosage = "2ml subcutaneous",
                Notes = "Annual Clostridial vaccination. No withdrawal required.",
                CreatedAt = today.AddDays(-30)
            },

            // Mandla — hoof trim + antibiotic, ACTIVE withdrawal (8 days remaining)
            new()
            {
                Id = Guid.Parse("70000000-0000-0000-0000-000000000004"),
                TenantId = DemoTenantId,
                AnimalId = BuckMandlaId,
                TreatmentDate = today.AddDays(-6),
                Medication = "Oxytetracycline LA",
                Dosage = "1ml per 10kg IM",
                WithdrawalPeriodDays = 14,
                VetName = "Dr. Mokoena",
                Notes = "Mild foot rot on front left. Hoof trimmed and treated.",
                CreatedAt = today.AddDays(-6)
            },

            // Lungile — deworming completed (no active withdrawal — market ready)
            new()
            {
                Id = Guid.Parse("70000000-0000-0000-0000-000000000005"),
                TenantId = DemoTenantId,
                AnimalId = KidLungileId,
                TreatmentDate = today.AddDays(-60),
                Medication = "Albendazole",
                Dosage = "5ml oral",
                WithdrawalPeriodDays = 14,
                Notes = "Pre-market deworming. Withdrawal period cleared.",
                CreatedAt = today.AddDays(-60)
            },

            // Lindiwe — vitamin supplement (no withdrawal)
            new()
            {
                Id = Guid.Parse("70000000-0000-0000-0000-000000000006"),
                TenantId = DemoTenantId,
                AnimalId = DoeLindiweId,
                TreatmentDate = today.AddDays(-20),
                Medication = "Vitamin ADE Injection",
                Dosage = "2ml IM",
                Notes = "Pre-kidding vitamin boost. No withdrawal.",
                CreatedAt = today.AddDays(-20)
            }
        };

        foreach (var log in medicalLogs)
        {
            await _db.PutAsync("medicalLogs", log);
        }
    }

    private async Task SeedCostEntriesAsync()
    {
        var today = DateTime.UtcNow.Date;

        var costEntries = new List<CostEntry>
        {
            // Feed costs (farm-wide)
            new()
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000001"),
                TenantId = DemoTenantId,
                Category = CostCategory.Feed,
                Amount = 1200.00m,
                Date = today.AddDays(-30),
                Description = "Lucerne bales x 20 — monthly feed supply",
                CreatedAt = today.AddDays(-30)
            },
            new()
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000002"),
                TenantId = DemoTenantId,
                Category = CostCategory.Feed,
                Amount = 450.00m,
                Date = today.AddDays(-15),
                Description = "Maize meal 50kg bag — supplementary feeding for pregnant does",
                CreatedAt = today.AddDays(-15)
            },
            new()
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000003"),
                TenantId = DemoTenantId,
                Category = CostCategory.Feed,
                Amount = 280.00m,
                Date = today.AddDays(-7),
                Description = "Mineral lick blocks x 4",
                CreatedAt = today.AddDays(-7)
            },

            // Veterinary costs (per animal)
            new()
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000004"),
                TenantId = DemoTenantId,
                AnimalId = BuckMandlaId,
                Category = CostCategory.Veterinary,
                Amount = 350.00m,
                Date = today.AddDays(-6),
                Description = "Dr. Mokoena — foot rot treatment and hoof trim",
                CreatedAt = today.AddDays(-6)
            },
            new()
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000005"),
                TenantId = DemoTenantId,
                AnimalId = KidSiphoId,
                Category = CostCategory.Veterinary,
                Amount = 180.00m,
                Date = today.AddDays(-9),
                Description = "Dr. Mokoena — scours treatment for Sipho",
                CreatedAt = today.AddDays(-9)
            },

            // Medication costs (farm-wide)
            new()
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000006"),
                TenantId = DemoTenantId,
                Category = CostCategory.Medication,
                Amount = 520.00m,
                Date = today.AddDays(-45),
                Description = "Ivermectin 500ml, Pulpy Kidney Vaccine x 20 doses — quarterly stock",
                CreatedAt = today.AddDays(-45)
            },

            // Labor costs
            new()
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000007"),
                TenantId = DemoTenantId,
                Category = CostCategory.Labor,
                Amount = 2000.00m,
                Date = today.AddDays(-1),
                Description = "Sbusiso — monthly herding and general farm labor",
                CreatedAt = today.AddDays(-1)
            },
            new()
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000008"),
                TenantId = DemoTenantId,
                Category = CostCategory.Labor,
                Amount = 2000.00m,
                Date = today.AddMonths(-1),
                Description = "Sbusiso — monthly herding and general farm labor",
                CreatedAt = today.AddMonths(-1)
            },

            // Equipment
            new()
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000009"),
                TenantId = DemoTenantId,
                Category = CostCategory.Equipment,
                Amount = 850.00m,
                Date = today.AddDays(-20),
                Description = "Ear tag applicator + 50 tags",
                CreatedAt = today.AddDays(-20)
            },

            // Cost attributed to sold animal
            new()
            {
                Id = Guid.Parse("80000000-0000-0000-0000-000000000010"),
                TenantId = DemoTenantId,
                AnimalId = SoldDoeLesediId,
                Category = CostCategory.Feed,
                Amount = 600.00m,
                Date = today.AddMonths(-3),
                Description = "Fattening feed for Lesedi before auction",
                CreatedAt = today.AddMonths(-3)
            }
        };

        foreach (var entry in costEntries)
        {
            await _db.PutAsync("costEntries", entry);
        }
    }

    private async Task SeedFarmSettingsAsync()
    {
        var settings = new FarmSettings
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
            CreatedAt = DateTime.UtcNow
        };

        await _db.PutAsync("farmSettings", settings);
    }
}
