using System.ComponentModel.DataAnnotations;

namespace ImbuziSmart.Shared.Auth;

public class InviteUserRequest
{
    [Required, EmailAddress]
    public string Email     { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName  { get; set; } = string.Empty;

    [Required]
    public string Password  { get; set; } = string.Empty;

    /// <summary>Role to assign: Manager or Viewer</summary>
    public string Role      { get; set; } = AppRoles.Viewer;
}
