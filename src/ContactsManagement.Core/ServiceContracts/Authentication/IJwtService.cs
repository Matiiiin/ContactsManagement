using ContactsManagement.Core.Domain.IdentityEntities;

namespace ContactsManagement.Core.ServiceContracts.Authentication;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user); 
}