namespace ImbuziSmart.Shared.Auth;

public static class AppRoles
{
    public const string Owner   = "Owner";
    public const string Manager = "Manager";
    public const string Viewer  = "Viewer";

    public static readonly string[] All = [Owner, Manager, Viewer];
}
