namespace Costealo.Dtos.Users;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public string TipoUsuario { get; set; } = "";
    public string TipoSuscripcion { get; set; } = "";
    public string? FotoPerfil { get; set; }
    
    // Decrypted payment info (only shown to the authenticated user)
    public string? TarjetaUltimos4Digitos { get; set; }
    public string? TarjetaCodigoSeguridad { get; set; }
    public string? TarjetaFechaVencimiento { get; set; }
}
