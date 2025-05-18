using Microsoft.AspNetCore.Identity;

namespace ContactsManagement.Core.Domain.IdentityEntities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FullName { get; set; }
}