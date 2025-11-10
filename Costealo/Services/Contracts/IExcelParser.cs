namespace Costealo.Services.Contracts;

public interface IExcelParser
{
    // Debe leer: ID | PRODUCTO | UNIDAD | PRECIO | MONEDA
    IEnumerable<(string id, string producto, string unidad, decimal precio, string moneda)> Read(Stream stream, string fileType);
}