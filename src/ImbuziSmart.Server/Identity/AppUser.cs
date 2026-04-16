using Microsoft.AspNetCore.Identity;
using ImbuziSmart.Shared.Entities;

namespace ImbuziSmart.Server.Identity;

public class AppUser : IdentityUser<Guid>
{
    public Guid     TenantId  { get; set; }
    public string   FirstName { get; set; } = string.Empty;
    public string   LastName  { get; set; } = string.Empty;
    public bool     IsActive  { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation — not used in WASM but useful for EF includes on server
    public Tenant Tenant { get; set; } = null!;
}
