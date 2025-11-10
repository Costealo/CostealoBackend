namespace Costealo.Dtos.Workbooks;

public record WorkbookItemUpsertDto(
    int? Id, int? PriceItemId,
    string Producto, string Unidad,
    decimal CantidadNeta, decimal MermaFactor,
    decimal PrecioUnit, decimal CostoAdicional);