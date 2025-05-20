using System.ComponentModel.DataAnnotations;

namespace ContactsManagement.Core.DTO.Identities;

public class LoginDTO
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [DataType(dataType: DataType.Password)]
    public string? Password { get; set; }
}