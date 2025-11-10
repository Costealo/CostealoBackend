using System.Security.Claims;
using Costealo.Data;
using Costealo.Dtos.Auth;
using Costealo.Models;
using Costealo.Services.Contracts;
using Costealo.Services.Security;
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
    public async Task<ActionResult<string>> Login(LoginDto dto)
    {
        var u = await db.Users.FirstOrDefaultAsync(x => x.Email == dto.Email && x.PasswordHash == dto.Password);
        if (u == null) return Unauthorized();
        var jwt = tokens.CreateToken(u.Id, u.Role, u.Email);
        return Ok(jwt);
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