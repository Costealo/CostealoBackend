using Costealo.Services.Contracts;

namespace Costealo.Services.Parsing;

public class ExcelParser : IExcelParser
{
    public IEnumerable<(string id, string producto, string unidad, decimal precio, string moneda)> Read(Stream stream, string fileType)
    {
        // Para demo: retornar vacío. Integra EPPlus/CsvHelper en tu equipo.
        return Enumerable.Empty<(string,string,string,decimal,string)>();
    }
}