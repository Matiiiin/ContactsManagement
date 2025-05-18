using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ContactsManagement.Core.DTO.Identities;

public class RegisterDTO
{
    [Required(ErrorMessage = "Person name cannot be empty.")]
    public string? PersonName { get; set; }
    
    [Required(ErrorMessage = "Email cannot be empty.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Phone number cannot be empty.")]
    [MaxLength(11 , ErrorMessage = "Invalid phone number. Max length 11 characters.")]
    [MinLength(11 , ErrorMessage = "Invalid phone number. Min length 11 characters.")]
    [RegularExpression("[^0-9]*$" , ErrorMessage = "Phone number should only contain numbers.") ]
    [DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }
    
    [Required(ErrorMessage = "Password cannot be empty.")]
    [DataType(DataType.Password)]
    [PasswordPropertyText]
    public string? Password { get; set; }
    
    [Required(ErrorMessage = "Confirm password cannot be empty.")]
    [PasswordPropertyText]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string? ConfirmPassword { get; set; }
}