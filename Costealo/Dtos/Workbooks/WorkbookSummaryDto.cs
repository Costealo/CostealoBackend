namespace Costealo.Dtos.Workbooks;

public class WorkbookSummaryDto
{
    public required int WorkbookId { get; set; }
    public required string Moneda { get; set; }
    public required int Unidades { get; set; }

    public required decimal CostoIngredientes { get; set; }
    public required decimal PesoTotal { get; set; }
    public required decimal PesoUnitario { get; set; }

    public required decimal CostosAdicionalesTotal { get; set; }
    public required decimal CostosOperativosUnit { get; set; }
    public required decimal CostoUnitario { get; set; }

    public required decimal CostoTotalUnitario { get; set; } // CTU
    public required decimal CostoTotal { get; set; }         // CT
    public required decimal CostoFinalUnitario { get; set; } // CFU
    public required decimal CostoFinal { get; set; }         // CFT

    public required decimal IvaPct { get; set; }
    public decimal? MargenObjetivoPct { get; set; }
    public decimal? PrecioVentaActualUnit { get; set; }
    public decimal? PrecioVentaSugeridoUnit { get; set; }
    public decimal? MargenActualPct { get; set; }
}