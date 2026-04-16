namespace ImbuziSmart.Shared.Auth;

public class LoginResponse
{
    public string   Token      { get; set; } = string.Empty;
    public string   Email      { get; set; } = string.Empty;
    public string   FullName   { get; set; } = string.Empty;
    public string   Role       { get; set; } = string.Empty;
    public Guid     TenantId   { get; set; }
    public string   TenantName { get; set; } = string.Empty;
    public DateTime ExpiresAt  { get; set; }
}
