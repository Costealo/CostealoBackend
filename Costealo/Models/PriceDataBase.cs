namespace Costealo.Models;

public class PriceDatabase
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Nombre { get; set; } = "";
    public string BlobUrl { get; set; } = "";
    public string FileName { get; set; } = "";
    public string FileType { get; set; } = "";
    public int RowCount { get; set; }
    public bool IsValidated { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? SourceUrl { get; set; } // URL for refreshable imports
    public DateTime? LastRefreshedAt { get; set; } // Last time data was refreshed from URL
    public List<PriceItem> Items { get; set; } = new();
}