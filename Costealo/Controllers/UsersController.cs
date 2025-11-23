using Costealo.Data;
using Costealo.Dtos.Users;
using Costealo.Models;
using Costealo.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Costealo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(AppDbContext db, IEncryptionService encryption) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
    {
        var users = await db.Users.ToListAsync();
        return Ok(users.Select(MapToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetUser(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null) return NotFound();
        return Ok(MapToDto(user));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> CreateUser(User user)
    {
        if (await db.Users.AnyAsync(u => u.Email == user.Email))
        {
            return BadRequest("Email ya registrado.");
        }

        // Encrypt payment data before saving
        if (!string.IsNullOrEmpty(user.TarjetaUltimos4Digitos))
            user.TarjetaUltimos4Digitos = encryption.Encrypt(user.TarjetaUltimos4Digitos);
        if (!string.IsNullOrEmpty(user.TarjetaCodigoSeguridad))
            user.TarjetaCodigoSeguridad = encryption.Encrypt(user.TarjetaCodigoSeguridad);
        if (!string.IsNullOrEmpty(user.TarjetaFechaVencimiento))
            user.TarjetaFechaVencimiento = encryption.Encrypt(user.TarjetaFechaVencimiento);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, MapToDto(user));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponseDto>> UpdateUserProfile(int id, UpdateUserProfileDto dto)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null) return NotFound();

        // Update only provided fields
        if (dto.Nombre != null) user.Nombre = dto.Nombre;
        if (dto.TipoUsuario != null) user.TipoUsuario = dto.TipoUsuario;
        if (dto.TipoSuscripcion != null) user.TipoSuscripcion = dto.TipoSuscripcion;
        if (dto.FotoPerfil != null) user.FotoPerfil = dto.FotoPerfil;

        // Encrypt and update payment data if provided
        if (dto.TarjetaUltimos4Digitos != null)
            user.TarjetaUltimos4Digitos = encryption.Encrypt(dto.TarjetaUltimos4Digitos);
        if (dto.TarjetaCodigoSeguridad != null)
            user.TarjetaCodigoSeguridad = encryption.Encrypt(dto.TarjetaCodigoSeguridad);
        if (dto.TarjetaFechaVencimiento != null)
            user.TarjetaFechaVencimiento = encryption.Encrypt(dto.TarjetaFechaVencimiento);

        await db.SaveChangesAsync();

        return Ok(MapToDto(user));
    }

    private UserResponseDto MapToDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Email = user.Email,
            Role = user.Role,
            TipoUsuario = user.TipoUsuario,
            TipoSuscripcion = user.TipoSuscripcion,
            FotoPerfil = user.FotoPerfil,
            // Decrypt payment data for response
            TarjetaUltimos4Digitos = !string.IsNullOrEmpty(user.TarjetaUltimos4Digitos) 
                ? encryption.Decrypt(user.TarjetaUltimos4Digitos) 
                : null,
            TarjetaCodigoSeguridad = !string.IsNullOrEmpty(user.TarjetaCodigoSeguridad) 
                ? encryption.Decrypt(user.TarjetaCodigoSeguridad) 
                : null,
            TarjetaFechaVencimiento = !string.IsNullOrEmpty(user.TarjetaFechaVencimiento) 
                ? encryption.Decrypt(user.TarjetaFechaVencimiento) 
                : null
        };
    }
}

