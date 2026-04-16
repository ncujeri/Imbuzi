using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ImbuziSmart.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddWeightRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Species = table.Column<int>(type: "int", nullable: false),
                    Breed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sex = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SireId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DamId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcquisitionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchasedFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcquisitionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SaleDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BuyerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FarmSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnableInbreedingCheck = table.Column<bool>(type: "bit", nullable: false),
                    EnableHeatTracking = table.Column<bool>(type: "bit", nullable: false),
                    HeatCycleDays = table.Column<int>(type: "int", nullable: false),
                    EnablePushNotifications = table.Column<bool>(type: "bit", nullable: false),
                    NotifyKiddingWatch = table.Column<bool>(type: "bit", nullable: false),
                    NotifyWeaning = table.Column<bool>(type: "bit", nullable: false),
                    NotifyWithdrawalExpiry = table.Column<bool>(type: "bit", nullable: false),
                    NotifyHeatCycle = table.Column<bool>(type: "bit", nullable: false),
                    KiddingWatchDaysBefore = table.Column<int>(type: "int", nullable: false),
                    HeatAlertDaysBefore = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "ZAR"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeatRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObservedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Signs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Intensity = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MatingRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuckId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualKiddingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatingRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Medication = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WithdrawalPeriodDays = table.Column<int>(type: "int", nullable: true),
                    VetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeightRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeighedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Kilograms = table.Column<double>(type: "float", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Scale"),
                    HeartGirthCm = table.Column<double>(type: "float", nullable: true),
                    BodyLengthCm = table.Column<double>(type: "float", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightRecords", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "AcquisitionDate", "AcquisitionType", "Breed", "BuyerName", "CreatedAt", "CreatedBy", "DamId", "DateOfBirth", "LastModifiedBy", "Name", "Notes", "PurchasedFrom", "SaleDate", "SalePrice", "Sex", "SireId", "Species", "Status", "Tag", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), null, null, "Boer", null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2023, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Themba", "Dominant buck. Excellent build, strong Boer genetics.", null, null, null, 0, null, 0, 0, "BK-001", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), null, null, "Boer Cross", null, new DateTime(2025, 12, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2023, 11, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mandla", "Young buck, good temperament. Ready for first breeding season.", null, null, null, 0, null, 0, 0, "BK-002", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("20000000-0000-0000-0000-000000000001"), null, null, "Boer", null, new DateTime(2025, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2022, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Nandi", "Top producer — 3 successful kiddings. Always twins.", null, null, null, 1, null, 0, 0, "DE-001", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("20000000-0000-0000-0000-000000000002"), null, null, "Boer", null, new DateTime(2025, 9, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2022, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lindiwe", "Daughter of Themba. Currently pregnant — due soon.", null, null, null, 1, new Guid("10000000-0000-0000-0000-000000000001"), 0, 0, "DE-002", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("20000000-0000-0000-0000-000000000003"), null, null, "Indigenous Veld", null, new DateTime(2025, 11, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2023, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Nomalusa", "Hardy doe, good forager. First pregnancy.", null, null, null, 1, null, 0, 0, "DE-003", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("20000000-0000-0000-0000-000000000004"), null, null, "Boer", null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2023, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thembi", "Daughter of Nandi. Good maternal instincts.", null, null, null, 1, null, 0, 0, "DE-004", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("20000000-0000-0000-0000-000000000005"), null, null, "Savanna", null, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Zanele", "White Savanna doe, heat observed recently.", null, null, null, 1, null, 0, 0, "DE-005", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("30000000-0000-0000-0000-000000000001"), null, null, "Boer", null, new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sipho", "Strong kid, growing well. Weaning approaching.", null, null, null, 0, new Guid("10000000-0000-0000-0000-000000000001"), 0, 0, "KD-001", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("30000000-0000-0000-0000-000000000002"), null, null, "Boer", null, new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ayanda", "Twin of Sipho. Slightly smaller but healthy.", null, null, null, 1, new Guid("10000000-0000-0000-0000-000000000001"), 0, 0, "KD-002", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("30000000-0000-0000-0000-000000000003"), null, null, "Boer Cross", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Busi", "Weaned successfully. Ready for next phase.", null, null, null, 1, new Guid("10000000-0000-0000-0000-000000000002"), 0, 0, "KD-003", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("30000000-0000-0000-0000-000000000004"), null, null, "Indigenous Veld", null, new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lungile", "Good weight, cleared withdrawal. Ready for market.", null, null, null, 0, null, 0, 3, "KD-004", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("40000000-0000-0000-0000-000000000001"), null, null, "Boer", null, new DateTime(2025, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTime(2024, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lesedi", "Sold at Vryburg auction — R3,200.", null, null, null, 1, null, 0, 1, "DE-006", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null }
                });

            migrationBuilder.InsertData(
                table: "CostEntries",
                columns: new[] { "Id", "Amount", "AnimalId", "Category", "CreatedAt", "CreatedBy", "Date", "Description", "LastModifiedBy", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("80000000-0000-0000-0000-000000000001"), 1200.00m, null, 0, new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Lucerne bales x 20 — monthly feed supply", null, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("80000000-0000-0000-0000-000000000002"), 450.00m, null, 0, new DateTime(2026, 3, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Maize meal 50kg bag — supplementary feeding for pregnant does", null, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("80000000-0000-0000-0000-000000000003"), 280.00m, null, 0, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Mineral lick blocks x 4", null, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("80000000-0000-0000-0000-000000000004"), 350.00m, new Guid("10000000-0000-0000-0000-000000000002"), 3, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Dr. Mokoena — foot rot treatment and hoof trim", null, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("80000000-0000-0000-0000-000000000005"), 180.00m, new Guid("30000000-0000-0000-0000-000000000001"), 3, new DateTime(2026, 4, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 4, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Dr. Mokoena — scours treatment for Sipho", null, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("80000000-0000-0000-0000-000000000006"), 520.00m, null, 1, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Ivermectin 500ml, Pulpy Kidney Vaccine x 20 doses — quarterly stock", null, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("80000000-0000-0000-0000-000000000007"), 2000.00m, null, 2, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Sbusiso — monthly herding and general farm labor", null, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("80000000-0000-0000-0000-000000000008"), 2000.00m, null, 2, new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Sbusiso — monthly herding and general farm labor", null, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("80000000-0000-0000-0000-000000000009"), 850.00m, null, 4, new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Ear tag applicator + 50 tags", null, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("80000000-0000-0000-0000-000000000010"), 600.00m, new Guid("40000000-0000-0000-0000-000000000001"), 0, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Fattening feed for Lesedi before auction", null, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null }
                });

            migrationBuilder.InsertData(
                table: "FarmSettings",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Currency", "EnableHeatTracking", "EnableInbreedingCheck", "EnablePushNotifications", "HeatAlertDaysBefore", "HeatCycleDays", "KiddingWatchDaysBefore", "LastModifiedBy", "NotifyHeatCycle", "NotifyKiddingWatch", "NotifyWeaning", "NotifyWithdrawalExpiry", "TenantId", "UpdatedAt" },
                values: new object[] { new Guid("90000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "ZAR", true, true, false, 2, 21, 5, null, true, true, true, true, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null });

            migrationBuilder.InsertData(
                table: "HeatRecords",
                columns: new[] { "Id", "AnimalId", "CreatedAt", "CreatedBy", "Intensity", "LastModifiedBy", "ObservedDate", "Signs", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000005"), new DateTime(2026, 3, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, null, new DateTime(2026, 3, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Flagging tail, frequent vocalization, mounting other does", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("60000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000005"), new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, 0, null, new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Mild flagging, slight swelling", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("60000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, null, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Standing heat, accepted buck", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("60000000-0000-0000-0000-000000000004"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, null, new DateTime(2026, 4, 6, 0, 0, 0, 0, DateTimeKind.Utc), "Restless, vocalizing, swollen vulva", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null }
                });

            migrationBuilder.InsertData(
                table: "MatingRecords",
                columns: new[] { "Id", "ActualKiddingDate", "BuckId", "CreatedAt", "CreatedBy", "DoeId", "LastModifiedBy", "MatingDate", "Notes", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2025, 8, 29, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("20000000-0000-0000-0000-000000000001"), null, new DateTime(2025, 8, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Twins born — both healthy. Sipho (male) and Ayanda (female).", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("50000000-0000-0000-0000-000000000002"), null, new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("20000000-0000-0000-0000-000000000002"), null, new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Utc), "First mating for Mandla. Lindiwe showing signs of nearing kidding.", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("50000000-0000-0000-0000-000000000003"), null, new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2025, 12, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("20000000-0000-0000-0000-000000000003"), null, new DateTime(2025, 12, 12, 0, 0, 0, 0, DateTimeKind.Utc), "First pregnancy for Nomalusa. Progressing well.", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null },
                    { new Guid("50000000-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2025, 8, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("20000000-0000-0000-0000-000000000004"), null, new DateTime(2025, 8, 4, 0, 0, 0, 0, DateTimeKind.Utc), "Single kid born — Busi (female). Easy delivery.", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null }
                });

            migrationBuilder.InsertData(
                table: "MedicalLogs",
                columns: new[] { "Id", "AnimalId", "CreatedAt", "CreatedBy", "Dosage", "LastModifiedBy", "Medication", "Notes", "TenantId", "TreatmentDate", "UpdatedAt", "VetName", "WithdrawalPeriodDays" },
                values: new object[,]
                {
                    { new Guid("70000000-0000-0000-0000-000000000001"), new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, "1ml per 10kg body weight", null, "Ivermectin", "Routine quarterly deworming. Withdrawal period expired.", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, "Dr. Mokoena", 35 },
                    { new Guid("70000000-0000-0000-0000-000000000002"), new Guid("30000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, "1ml per 5kg", null, "Sulfadimidine", "Mild scours, responding well to treatment.", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new DateTime(2026, 4, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, "Dr. Mokoena", 14 },
                    { new Guid("70000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, "2ml subcutaneous", null, "Pulpy Kidney Vaccine", "Annual Clostridial vaccination. No withdrawal required.", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null },
                    { new Guid("70000000-0000-0000-0000-000000000004"), new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "1ml per 10kg IM", null, "Oxytetracycline LA", "Mild foot rot on front left. Hoof trimmed and treated.", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "Dr. Mokoena", 14 },
                    { new Guid("70000000-0000-0000-0000-000000000005"), new Guid("30000000-0000-0000-0000-000000000004"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, "5ml oral", null, "Albendazole", "Pre-market deworming. Withdrawal period cleared.", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, null, 14 },
                    { new Guid("70000000-0000-0000-0000-000000000006"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "2ml IM", null, "Vitamin ADE Injection", "Pre-kidding vitamin boost. No withdrawal.", new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animals_TenantId_Tag",
                table: "Animals",
                columns: new[] { "TenantId", "Tag" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostEntries_TenantId_AnimalId",
                table: "CostEntries",
                columns: new[] { "TenantId", "AnimalId" });

            migrationBuilder.CreateIndex(
                name: "IX_FarmSettings_TenantId",
                table: "FarmSettings",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeatRecords_TenantId_AnimalId",
                table: "HeatRecords",
                columns: new[] { "TenantId", "AnimalId" });

            migrationBuilder.CreateIndex(
                name: "IX_MatingRecords_TenantId_DoeId_MatingDate",
                table: "MatingRecords",
                columns: new[] { "TenantId", "DoeId", "MatingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalLogs_TenantId_AnimalId",
                table: "MedicalLogs",
                columns: new[] { "TenantId", "AnimalId" });

            migrationBuilder.CreateIndex(
                name: "IX_WeightRecords_TenantId_AnimalId",
                table: "WeightRecords",
                columns: new[] { "TenantId", "AnimalId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Animals");

            migrationBuilder.DropTable(
                name: "CostEntries");

            migrationBuilder.DropTable(
                name: "FarmSettings");

            migrationBuilder.DropTable(
                name: "HeatRecords");

            migrationBuilder.DropTable(
                name: "MatingRecords");

            migrationBuilder.DropTable(
                name: "MedicalLogs");

            migrationBuilder.DropTable(
                name: "WeightRecords");
        }
    }
}
