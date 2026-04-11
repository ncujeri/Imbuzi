using Microsoft.AspNetCore.Mvc;

namespace ImbuziSmart.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    [HttpPost("push")]
    public IActionResult Push([FromBody] object payload)
    {
        // TODO: Accept SyncPayload, apply changes to database
        return Ok(new { success = true, message = "Sync push received." });
    }

    [HttpGet("pull")]
    public IActionResult Pull([FromQuery] DateTime? since)
    {
        // TODO: Return changes since the given timestamp
        return Ok(new { success = true, data = new { animals = Array.Empty<object>(), matingRecords = Array.Empty<object>(), heatRecords = Array.Empty<object>(), medicalLogs = Array.Empty<object>(), costEntries = Array.Empty<object>() } });
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
