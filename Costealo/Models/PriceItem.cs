namespace Costealo.Models;

public class PriceItem
{
    public int Id { get; set; }
    public int PriceDatabaseId { get; set; }
    public string ExtId { get; set; } = "";         // ID del archivo
    public string Producto { get; set; } = "";
    public string Unidad { get; set; } = "g";       // g, kg, ml, L, unid
    public decimal Precio { get; set; }             // por unidad indicada
    public string Moneda { get; set; } = "BOB";     // ISO 4217
}