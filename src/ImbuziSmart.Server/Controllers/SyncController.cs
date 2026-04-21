using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using ImbuziSmart.Server.Data;
using ImbuziSmart.Shared.Auth;
using ImbuziSmart.Shared.Entities;
using ImbuziSmart.Shared.Enums;
using ImbuziSmart.Shared.Sync;

namespace ImbuziSmart.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SyncController : ControllerBase
{
    private readonly ImbuziDbContext _db;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SyncController(ImbuziDbContext db) => _db = db;

    // ── Push: client → server ─────────────────────────────────────────────────

    [HttpPost("push")]
    public async Task<ActionResult<SyncPushResult>> Push([FromBody] SyncPushRequest request)
    {
        var result = new SyncPushResult();

        foreach (var entry in request.Changes)
        {
            try
            {
                await ApplyEntryAsync(entry);
                result.Applied++;
            }
            catch (Exception ex)
            {
                result.Errors.Add(new SyncEntryError
                {
                    QueueId    = entry.QueueId,
                    EntityType = entry.EntityType,
                    EntityId   = entry.EntityId,
                    Error      = ex.Message
                });
            }
        }

        result.Success = result.Errors.Count == 0;
        return Ok(result);
    }

    // ── Pull: server → client ─────────────────────────────────────────────────

    /// <summary>
    /// Returns all entities belonging to the authenticated user's tenant.
    /// The ImbuziDbContext query filter automatically scopes results by tenant_id.
    /// </summary>
    [HttpGet("pull")]
    public async Task<ActionResult<SyncPullResponse>> Pull()
    {
        var response = new SyncPullResponse
        {
            Animals       = await _db.Animals.ToListAsync(),
            WeightRecords = await _db.WeightRecords.ToListAsync(),
            MatingRecords = await _db.MatingRecords.ToListAsync(),
            HeatRecords   = await _db.HeatRecords.ToListAsync(),
            MedicalLogs   = await _db.MedicalLogs.ToListAsync(),
            CostEntries   = await _db.CostEntries.ToListAsync()
        };

        response.Success = true;
        return Ok(response);
    }

    // ── Health ────────────────────────────────────────────────────────────────

    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health() =>
        Ok(new { status = "healthy", timestamp = DateTime.UtcNow });

    // ── Private helpers ──────────────────────────────────────────────────────

    private async Task ApplyEntryAsync(SyncEntry entry)
    {
        switch (entry.EntityType)
        {
            case "animals":
                await UpsertOrDeleteAnimalAsync(entry);
                break;
            case "weightRecords":
                await UpsertOrDeleteAsync<WeightRecord>(entry);
                break;
            case "matingRecords":
                await UpsertOrDeleteAsync<MatingRecord>(entry);
                break;
            case "heatRecords":
                await UpsertOrDeleteAsync<HeatRecord>(entry);
                break;
            case "medicalLogs":
                await UpsertOrDeleteAsync<MedicalLog>(entry);
                break;
            case "costEntries":
                await UpsertOrDeleteAsync<CostEntry>(entry);
                break;
            default:
                throw new InvalidOperationException($"Unknown entity type: {entry.EntityType}");
        }
    }

    private async Task UpsertOrDeleteAnimalAsync(SyncEntry entry)
    {
        if (entry.Action == "delete" || User.IsInRole(AppRoles.Owner))
        {
            await UpsertOrDeleteAsync<Animal>(entry);
            return;
        }

        var oldAnimal = await _db.Animals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == entry.EntityId);

        await UpsertOrDeleteAsync<Animal>(entry);

        if (string.IsNullOrEmpty(entry.Payload)) return;
        var incoming = JsonSerializer.Deserialize<Animal>(entry.Payload, _json);
        if (incoming == null) return;

        var isNotifiableStatus = incoming.Status is AnimalStatus.Sold or AnimalStatus.Deceased;
        var statusChanged = oldAnimal?.Status != incoming.Status;

        if (isNotifiableStatus && statusChanged)
            await CreateAnimalNotificationAsync(incoming);
    }

    private async Task CreateAnimalNotificationAsync(Animal animal)
    {
        var changerName = User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.FindFirst("email")?.Value
            ?? "A team member";

        if (!Guid.TryParse(User.FindFirst("tenant_id")?.Value, out var tenantId)) return;

        var label = string.IsNullOrEmpty(animal.Name) ? animal.Tag : $"{animal.Name} ({animal.Tag})";

        string title, message;
        if (animal.Status == AnimalStatus.Sold)
        {
            var extra = animal.SalePrice.HasValue
                ? $" for R{animal.SalePrice:N2}" + (!string.IsNullOrEmpty(animal.BuyerName) ? $" to {animal.BuyerName}" : "")
                : (!string.IsNullOrEmpty(animal.BuyerName) ? $" to {animal.BuyerName}" : "");
            title = $"{label} Sold";
            message = $"{changerName} recorded {label} as sold{extra}.";
        }
        else
        {
            title = $"{label} Deceased";
            message = $"{changerName} recorded {label} as deceased.";
        }

        _db.Notifications.Add(new AppNotification
        {
            TenantId   = tenantId,
            Title      = title,
            Message    = message,
            EntityType = "Animal",
            EntityId   = animal.Id,
            IsRead     = false,
            CreatedAt  = DateTime.UtcNow,
            CreatedBy  = changerName
        });
        await _db.SaveChangesAsync();
    }

    private async Task UpsertOrDeleteAsync<TEntity>(SyncEntry entry)
        where TEntity : BaseEntity
    {
        if (entry.Action == "delete")
        {
            var existing = await _db.Set<TEntity>()
                                    .IgnoreQueryFilters()
                                    .FirstOrDefaultAsync(e => e.Id == entry.EntityId);
            if (existing is not null)
            {
                _db.Set<TEntity>().Remove(existing);
                await _db.SaveChangesAsync();
            }
            return;
        }

        // Upsert
        if (string.IsNullOrEmpty(entry.Payload))
            throw new InvalidOperationException($"Payload is required for upsert (entityType={entry.EntityType}, id={entry.EntityId})");

        var incoming = JsonSerializer.Deserialize<TEntity>(entry.Payload, _json)
            ?? throw new InvalidOperationException($"Failed to deserialize {entry.EntityType}");

        var existing2 = await _db.Set<TEntity>()
                                  .IgnoreQueryFilters()
                                  .FirstOrDefaultAsync(e => e.Id == entry.EntityId);
        if (existing2 is null)
        {
            _db.Set<TEntity>().Add(incoming);
        }
        else
        {
            _db.Entry(existing2).CurrentValues.SetValues(incoming);
        }

        await _db.SaveChangesAsync();
    }
}
