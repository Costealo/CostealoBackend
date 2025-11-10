using Costealo.Data;
using Costealo.Dtos.Databases;
using Costealo.Models;
using Costealo.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Costealo.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DatabasesController(AppDbContext db, IBlobService blob, IExcelParser parser) : ControllerBase
{
  [HttpPost("upload")]
  public async Task<ActionResult<PriceDatabaseDto>> Upload(IFormFile file)
  {
    if (file is null || file.Length == 0) return BadRequest("Archivo requerido.");
    var blobUrl = await blob.UploadAsync(file.OpenReadStream(), file.ContentType, file.FileName);

    var rows = parser.Read(file.OpenReadStream(), System.IO.Path.GetExtension(file.FileName));
    var pd = new PriceDatabase { Nombre = System.IO.Path.GetFileNameWithoutExtension(file.FileName), FileName = file.FileName, FileType = "xlsx/csv", BlobUrl = blobUrl };
    db.PriceDatabases.Add(pd);
    await db.SaveChangesAsync();

    int count = 0;
    foreach (var (id, producto, unidad, precio, moneda) in rows)
    {
      db.PriceItems.Add(new PriceItem { PriceDatabaseId = pd.Id, ExtId = id.Trim(), Producto = producto.Trim().ToUpperInvariant(), Unidad = unidad.Trim().ToLowerInvariant(), Precio = precio, Moneda = moneda.Trim().ToUpperInvariant() });
      count++;
    }
    pd.RowCount = count; pd.IsValidated = count > 0; await db.SaveChangesAsync();
    return Ok(new PriceDatabaseDto(pd.Id, pd.Nombre, pd.FileName, pd.FileType, pd.RowCount, pd.BlobUrl));
  }

  [HttpGet("{id}/items")]
  public async Task<ActionResult<IEnumerable<PriceItemDto>>> Items(int id, string? search = null, int page = 1, int pageSize = 20)
  {
    var q = db.PriceItems.Where(x => x.PriceDatabaseId == id);
    if (!string.IsNullOrWhiteSpace(search))
      q = q.Where(x => x.Producto.Contains(search.ToUpper()) || x.ExtId.Contains(search));
    var list = await q.OrderBy(x => x.Producto).Skip((page - 1) * pageSize).Take(pageSize)
      .Select(x => new PriceItemDto(x.Id, x.PriceDatabaseId, x.ExtId, x.Producto, x.Unidad, x.Precio, x.Moneda)).ToListAsync();
    return Ok(list);
  }
}
