using System.ComponentModel.DataAnnotations;

namespace BlazorHosted.Data;

public class BlazorCustomer
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }
}
