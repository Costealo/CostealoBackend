namespace Costealo.Models;

public class WorkbookItem
{
    public int Id { get; set; }
    public int WorkbookId { get; set; }
    public int? PriceItemId { get; set; } // link con base
    public string Producto { get; set; } = "";
    public string Unidad { get; set; } = "g"; // g, ml, unid
    public decimal CantidadNeta { get; set; }
    public decimal MermaFactor { get; set; } = 1m;
    public decimal PrecioUnit { get; set; } // ya convertido a Moneda del workbook
    public decimal CostoAdicional { get; set; }
    public decimal Subtotal { get; set; } // CantidadNeta * Merma * Precio + CostoAdicional
}