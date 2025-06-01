using ContactsManagement.Core.Domain.IdentityEntities;
using ContactsManagement.Core.DTO.JwtToken;

namespace ContactsManagement.Core.ServiceContracts.Authentication;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user); 
    string GenerateRefreshToken();
    Task<RefreshTokenGenerateDTO> GenerateTokenFromRefreshToken(ApplicationUser user , string refreshToken);
}