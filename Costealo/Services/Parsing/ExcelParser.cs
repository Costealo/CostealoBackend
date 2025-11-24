using ClosedXML.Excel;
using Costealo.Services.Contracts;

namespace Costealo.Services.Parsing;

public class ExcelParser : IExcelParser
{
    public IEnumerable<(string id, string producto, string unidad, decimal precio, string moneda)> Read(Stream stream, string fileType)
    {
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1); // Leer la primera hoja
        var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Saltar encabezado

        foreach (var row in rows)
        {
            // Asumimos orden: ID, Producto, Unidad, Precio, Moneda
            // O buscamos por nombre de columna si fuera necesario. Para MVP, usamos índices fijos (1-based en ClosedXML)
            
            var id = row.Cell(1).GetValue<string>();
            var producto = row.Cell(2).GetValue<string>();
            var unidad = row.Cell(3).GetValue<string>();
            
            // Precio puede venir como texto o número
            var precioRaw = row.Cell(4).GetValue<string>();
            decimal.TryParse(precioRaw, out var precio); // Validación más estricta se hace en ImportValidationService

            var moneda = row.Cell(5).GetValue<string>();

            yield return (id, producto, unidad, precio, moneda);
        }
    }
}