using System.ComponentModel.DataAnnotations;

namespace ContactsManagement.Core.DTO.Identities;

public class RefreshTokenDTO
{
    [Required]
    public string? Token { get; set; }
    
    [Required]
    public string? Refreshtoken { get; set; }
}