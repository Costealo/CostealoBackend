namespace Costealo.Models;

public class Workbook
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Nombre { get; set; } = "";
    public int? PriceDatabaseId { get; set; }
    public int Unidades { get; set; } = 1;
    public decimal IvaPct { get; set; } = 0.13m;
    public decimal? MargenObjetivoPct { get; set; }
    public decimal? CostoOperativosPct { get; set; }
    public decimal? CostoOperativosMonto { get; set; }
    public decimal? PrecioVentaActualUnit { get; set; }
    public string Moneda { get; set; } = "BOB";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<WorkbookItem> Items { get; set; } = new();
}