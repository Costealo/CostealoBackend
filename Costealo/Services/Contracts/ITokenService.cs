namespace Costealo.Services.Contracts;
public interface ITokenService
{
    string CreateToken(int userId, string role, string email);
}