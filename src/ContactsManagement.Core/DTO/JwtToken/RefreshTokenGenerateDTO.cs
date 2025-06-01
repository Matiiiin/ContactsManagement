using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ContactsManagement.Core.DTO.JwtToken;

public class RefreshTokenGenerateDTO
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}