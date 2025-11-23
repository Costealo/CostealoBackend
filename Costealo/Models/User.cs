namespace Costealo.Models;

public class User
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "User"; // "User" | "Admin"
    
    // Profile fields
    public string TipoUsuario { get; set; } = ""; // "Empresa" | "Independiente"
    public string TipoSuscripcion { get; set; } = "Free"; // "Free" | "Premium" | "Enterprise"
    public string? FotoPerfil { get; set; } // URL or path to profile photo
    
    // Encrypted payment fields (stored as encrypted Base64 strings)
    public string? TarjetaUltimos4Digitos { get; set; } // Encrypted
    public string? TarjetaCodigoSeguridad { get; set; } // Encrypted CVV
    public string? TarjetaFechaVencimiento { get; set; } // Encrypted (MM/YY)
}