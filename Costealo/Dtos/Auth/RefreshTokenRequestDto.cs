using System.ComponentModel.DataAnnotations;

namespace Costealo.Dtos.Auth;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = "";
}
