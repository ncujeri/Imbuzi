using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ImbuziSmart.Server.Data;
using ImbuziSmart.Shared.Entities;
using ImbuziSmart.Shared.Sync;

namespace ImbuziSmart.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly ImbuziDbContext _db;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SyncController(ImbuziDbContext db) => _db = db;

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

    [HttpGet("pull")]
    public IActionResult Pull([FromQuery] DateTime? since)
    {
        // TODO: Return changes since the given timestamp
        return Ok(new
        {
            success = true,
            data = new
            {
                animals        = Array.Empty<object>(),
                weightRecords  = Array.Empty<object>(),
                matingRecords  = Array.Empty<object>(),
                heatRecords    = Array.Empty<object>(),
                medicalLogs    = Array.Empty<object>(),
                costEntries    = Array.Empty<object>()
            }
        });
    }

    [HttpGet("health")]
    public IActionResult Health() =>
        Ok(new { status = "healthy", timestamp = DateTime.UtcNow });

    // ── Private helpers ──────────────────────────────────────────────────────

    private async Task ApplyEntryAsync(SyncEntry entry)
    {
        switch (entry.EntityType)
        {
            case "animals":
                await UpsertOrDeleteAsync<Animal>(entry);
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

    private async Task UpsertOrDeleteAsync<TEntity>(SyncEntry entry)
        where TEntity : BaseEntity
    {
        if (entry.Action == "delete")
        {
            var existing = await _db.Set<TEntity>()
                                    .IgnoreQueryFilters()   // bypass tenant filter for direct lookup
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
