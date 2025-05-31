namespace ContactsManagement.Core.DTO.Identities;

public class JwtRegisterResponseDTO
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}