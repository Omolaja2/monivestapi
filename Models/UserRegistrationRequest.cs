using System.ComponentModel.DataAnnotations;

namespace monivestuserapi.Models;

public class UserRegistrationRequest
{
    [Required(ErrorMessage = "Please enter your first name and last name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email address")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please provide correct phone number")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Must be at least 8 characters long.")]
    public string Password { get; set; } = string.Empty; 

    [Required(ErrorMessage = "Re-enter your password to confirm")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;


}
