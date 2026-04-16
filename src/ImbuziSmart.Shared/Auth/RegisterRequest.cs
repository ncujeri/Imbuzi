using System.ComponentModel.DataAnnotations;

namespace ImbuziSmart.Shared.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Farm name is required")]
    public string FarmName  { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName  { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email     { get; set; } = string.Empty;

    [Required, MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password  { get; set; } = string.Empty;
}
