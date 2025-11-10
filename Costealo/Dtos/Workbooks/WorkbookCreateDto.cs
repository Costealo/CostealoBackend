namespace Costealo.Dtos.Workbooks;

public record WorkbookCreateDto(
    string Nombre, int? PriceDatabaseId,
    int Unidades, decimal IvaPct,
    decimal? MargenObjetivoPct,
    decimal? CostoOperativosPct, decimal? CostoOperativosMonto,
    decimal? PrecioVentaActualUnit,
    string Moneda);