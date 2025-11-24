using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Costealo.Services.Contracts;

namespace Costealo.Services.Security;

public class TokenService(IConfiguration cfg) : ITokenService
{
    public string CreateToken(int userId, string role, string email)
    {
        var rawKey = cfg["Jwt:Key"] ?? "dev-key-please-change";
        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(rawKey);
        }
        catch
        {
            keyBytes = Encoding.UTF8.GetBytes(rawKey);
        }

        var key = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Email, email)
        };

        var token = new JwtSecurityToken(
            issuer: cfg["Jwt:Issuer"],
            audience: cfg["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}