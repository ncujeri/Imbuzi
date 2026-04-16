namespace ImbuziSmart.Server.Identity;

public class JwtOptions
{
    public string Secret     { get; set; } = string.Empty;
    public string Issuer     { get; set; } = "ImbuziSmart";
    public string Audience   { get; set; } = "ImbuziSmart";
    public int    ExpiryDays { get; set; } = 30;
}
