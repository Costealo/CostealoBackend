namespace Costealo.Dtos.Workbooks;

public record WorkbookUpdateDto(
    int? Unidades, decimal? IvaPct,
    decimal? MargenObjetivoPct,
    decimal? CostoOperativosPct, decimal? CostoOperativosMonto,
    decimal? PrecioVentaActualUnit,
    string? Moneda, int? PriceDatabaseId);