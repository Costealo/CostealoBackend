namespace Costealo.Dtos.Databases;

public record PriceItemDto(int Id, int PriceDatabaseId, string ExtId, string Producto, string Unidad, decimal Precio, string Moneda);