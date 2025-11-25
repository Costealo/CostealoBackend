using System.Security.Claims;
using Costealo.Data;
using Costealo.Dtos.Auth;
using Costealo.Models;
using Costealo.Services.Contracts;
using Costealo.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Costealo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, ITokenService tokens) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        if (await db.Users.AnyAsync(u => u.Email == dto.Email)) return BadRequest("Email ya registrado.");
        var user = new User { Nombre = dto.Nombre, Email = dto.Email, PasswordHash = dto.Password }; // Hash real pendiente
        db.Users.Add(user); await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        var u = await db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email && x.PasswordHash == dto.Password);
        if (u == null) return Unauthorized();

        var (accessToken, expiresIn) = tokens.CreateToken(u);
        var refreshToken = tokens.GenerateRefreshToken(u.Id);

        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync();

        return Ok(new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresIn = expiresIn
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> Refresh(RefreshTokenRequestDto dto)
    {
        var existingToken = await db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);

        if (existingToken == null)
            return Unauthorized(new { message = "Refresh token inválido" });

        if (existingToken.IsRevoked)
        {
            // Token rotation security: si usan un token revocado, revocamos todo por seguridad
            // (Opcional: implementar lógica de revocar toda la cadena)
            return Unauthorized(new { message = "Token revocado" });
        }

        if (existingToken.IsExpired)
            return Unauthorized(new { message = "Token expirado" });

        // Token Rotation: Revocar el anterior y generar uno nuevo
        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.ReplacedByToken = "NEW_GENERATED"; // Se podría guardar el hash del nuevo

        var user = existingToken.User;
        var (newAccessToken, expiresIn) = tokens.CreateToken(user);
        var newRefreshToken = tokens.GenerateRefreshToken(user.Id);

        existingToken.ReplacedByToken = newRefreshToken.Token;
        db.RefreshTokens.Add(newRefreshToken);
        await db.SaveChangesAsync();

        return Ok(new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresIn = expiresIn
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout(RefreshTokenRequestDto dto)
    {
        // Revocar el token específico enviado
        var token = await db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);
        if (token != null && token.IsActive)
        {
            token.RevokedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
        return Ok(new { message = "Sesión cerrada exitosamente" });
    }

    [Authorize]
    [HttpPost("revoke-all")]
    public async Task<ActionResult> RevokeAll()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userTokens = await db.RefreshTokens
            .Where(r => r.UserId == userId && r.RevokedAt == null)
            .ToListAsync();

        foreach (var t in userTokens)
        {
            t.RevokedAt = DateTime.UtcNow;
        }
        await db.SaveChangesAsync();

        return Ok(new { message = "Todas las sesiones han sido revocadas" });
    }

    [HttpPost("registerAdmin")]
    [SwaggerOnly]
    public async Task<ActionResult> RegisterAdmin(RegisterDto dto)
    {
        if (await db.Users.AnyAsync(u => u.Email == dto.Email)) return BadRequest("Email ya registrado.");
        var user = new User { Nombre = dto.Nombre, Email = dto.Email, PasswordHash = dto.Password, Role = "Admin" };
        db.Users.Add(user); await db.SaveChangesAsync(); return Ok();
    }
}