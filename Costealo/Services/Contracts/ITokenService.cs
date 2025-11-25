using Costealo.Models;

namespace Costealo.Services.Contracts;
public interface ITokenService
{
    (string Token, int ExpiresIn) CreateToken(User user);
    RefreshToken GenerateRefreshToken(int userId);
}