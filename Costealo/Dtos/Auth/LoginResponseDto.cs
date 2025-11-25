namespace Costealo.Dtos.Auth;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public int ExpiresIn { get; set; } // En segundos
    public string TokenType { get; set; } = "Bearer";
}
