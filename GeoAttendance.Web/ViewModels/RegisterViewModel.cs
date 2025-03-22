using System.ComponentModel.DataAnnotations;

namespace GeoAttendance.Web.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "Username")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; }

    [Required]
    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Required]
    [Display(Name = "Role")]
    public string Role { get; set; }

    // View model specific properties
    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; }

    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Display(Name = "Phone Number")]
    [Phone]
    public string PhoneNumber { get; set; }

    [Display(Name = "Department")]
    public string Department { get; set; }
}