using Microsoft.AspNetCore.Mvc;

namespace ImbuziSmart.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    [HttpPost("push")]
    public IActionResult Push()
    {
        // TODO: Accept SyncPayload, apply changes to SQL Server
        return Ok(new { success = true, message = "Sync push endpoint — not yet implemented." });
    }

    [HttpGet("pull")]
    public IActionResult Pull([FromQuery] DateTime? since)
    {
        // TODO: Return all records modified since the given timestamp
        return Ok(new { success = true, message = "Sync pull endpoint — not yet implemented.", since });
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
