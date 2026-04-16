namespace ImbuziSmart.Shared.Entities;

public class Tenant
{
    public Guid   Id           { get; set; } = Guid.NewGuid();
    public string Name         { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public bool   IsActive     { get; set; } = true;
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
}
