namespace Costealo.Dtos.Users;

public class UpdateUserProfileDto
{
    public string? Nombre { get; set; }
    public string? TipoUsuario { get; set; } // "Empresa" | "Independiente"
    public string? TipoSuscripcion { get; set; }
    public string? FotoPerfil { get; set; }
    
    // Payment info (will be encrypted before storage)
    public string? TarjetaUltimos4Digitos { get; set; }
    public string? TarjetaCodigoSeguridad { get; set; }
    public string? TarjetaFechaVencimiento { get; set; } // Format: MM/YY
}
