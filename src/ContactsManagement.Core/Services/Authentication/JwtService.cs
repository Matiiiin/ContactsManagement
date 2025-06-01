using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ContactsManagement.Core.Domain.IdentityEntities;
using ContactsManagement.Core.ServiceContracts.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ContactsManagement.Core.Services.Authentication;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly ILogger<JwtService> _logger;
    private readonly SymmetricSecurityKey _key;

    public JwtService(IConfiguration config , ILogger<JwtService> logger)
    {
        _config = config;
        _logger = logger;
        _key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["Jwt:SigningKey"]!));
    }
    public string GenerateToken(ApplicationUser user)
    {

        var claims = new List<Claim>
        {            
            new(JwtRegisteredClaimNames.NameId, user.Id.ToString()!),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iss, _config["Jwt:Issuer"]!),
            new(JwtRegisteredClaimNames.Aud, _config["Jwt:Audience"]!),
        };
        
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpirationMinutes"])),
            SigningCredentials = credentials,
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(descriptor);
        return tokenHandler.WriteToken(token);

    }
    
}