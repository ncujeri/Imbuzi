using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImbuziSmart.Server.Data;
using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "OwnerOnly")]
public class NotificationsController : ControllerBase
{
    private readonly ImbuziDbContext _db;

    public NotificationsController(ImbuziDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<AppNotification>>> GetUnread()
    {
        var notifications = await _db.Notifications
            .Where(n => !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return Ok(notifications);
    }

    [HttpPost("mark-read")]
    public async Task<IActionResult> MarkRead([FromBody] List<Guid> ids)
    {
        var items = await _db.Notifications
            .Where(n => ids.Contains(n.Id))
            .ToListAsync();
        foreach (var n in items) n.IsRead = true;
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllRead()
    {
        var items = await _db.Notifications
            .Where(n => !n.IsRead)
            .ToListAsync();
        foreach (var n in items) n.IsRead = true;
        await _db.SaveChangesAsync();
        return Ok();
    }
}
