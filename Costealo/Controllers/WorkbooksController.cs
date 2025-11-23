using System.Security.Claims;
using Costealo.Data;
using Costealo.Dtos.Workbooks;
using Costealo.Models;
using Costealo.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Costealo.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WorkbooksController(AppDbContext db, IWorkbookService svc, IUnitConversionService units)
  : ControllerBase
{
  private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

  // GET: api/workbooks
  [HttpGet]
  public async Task<ActionResult<IEnumerable<Workbook>>> GetAll()
  {
    var workbooks = await db.Workbooks
      .Where(w => w.UserId == CurrentUserId)
      .OrderByDescending(w => w.CreatedAt)
      .ToListAsync();
    return Ok(workbooks);
  }

  // GET: api/workbooks/5
  [HttpGet("{id}")]
  public async Task<ActionResult> Get(int id)
  {
    var w = await db.Workbooks.Include(x => x.Items)
      .FirstOrDefaultAsync(x => x.Id == id && x.UserId == CurrentUserId);
    if (w == null) return NotFound();
    return Ok(w);
  }

  // POST: api/workbooks
  [HttpPost]
  public async Task<ActionResult<int>> Create(WorkbookCreateDto dto)
  {
    var w = new Workbook
    {
      UserId = CurrentUserId,
      Nombre = dto.Nombre,
      PriceDatabaseId = dto.PriceDatabaseId,
      Unidades = dto.Unidades,
      IvaPct = dto.IvaPct,
      MargenObjetivoPct = dto.MargenObjetivoPct,
      CostoOperativosPct = dto.CostoOperativosPct,
      CostoOperativosMonto = dto.CostoOperativosMonto,
      PrecioVentaActualUnit = dto.PrecioVentaActualUnit,
      Moneda = dto.Moneda
    };
    db.Workbooks.Add(w);
    await db.SaveChangesAsync();
    return Ok(w.Id);
  }

  // PUT: api/workbooks/5
  [HttpPut("{id}")]
  public async Task<ActionResult> Update(int id, WorkbookUpdateDto dto)
  {
    var w = await db.Workbooks.FirstOrDefaultAsync(x => x.Id == id && x.UserId == CurrentUserId);
    if (w == null) return NotFound();

    if (dto.Unidades.HasValue) w.Unidades = dto.Unidades.Value;
    if (dto.IvaPct.HasValue) w.IvaPct = dto.IvaPct.Value;
    if (dto.MargenObjetivoPct.HasValue) w.MargenObjetivoPct = dto.MargenObjetivoPct.Value;
    if (dto.CostoOperativosPct.HasValue) w.CostoOperativosPct = dto.CostoOperativosPct.Value;
    if (dto.CostoOperativosMonto.HasValue) w.CostoOperativosMonto = dto.CostoOperativosMonto.Value;
    if (dto.PrecioVentaActualUnit.HasValue) w.PrecioVentaActualUnit = dto.PrecioVentaActualUnit.Value;
    if (!string.IsNullOrWhiteSpace(dto.Moneda)) w.Moneda = dto.Moneda!;
    if (dto.PriceDatabaseId.HasValue) w.PriceDatabaseId = dto.PriceDatabaseId;
    await db.SaveChangesAsync();
    return Ok();
  }

  // DELETE: api/workbooks/5
  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(int id)
  {
    var w = await db.Workbooks
      .Include(x => x.Items)
      .FirstOrDefaultAsync(x => x.Id == id && x.UserId == CurrentUserId);
    if (w == null) return NotFound();

    db.WorkbookItems.RemoveRange(w.Items);
    db.Workbooks.Remove(w);
    await db.SaveChangesAsync();

    return NoContent();
  }

  // PUT: api/workbooks/5/items/bulk
  [HttpPut("{id}/items/bulk")]
  public async Task<ActionResult> UpsertItems(int id, List<WorkbookItemUpsertDto> items)
  {
    var w = await db.Workbooks.FirstOrDefaultAsync(x => x.Id == id && x.UserId == CurrentUserId);
    if (w == null) return NotFound();

    foreach (var it in items)
    {
      WorkbookItem? row = null;
      if (it.Id.HasValue)
        row = await db.WorkbookItems.FirstOrDefaultAsync(r => r.Id == it.Id.Value && r.WorkbookId == id);
      if (row == null)
      {
        row = new WorkbookItem { WorkbookId = id };
        db.WorkbookItems.Add(row);
      }

      row.PriceItemId = it.PriceItemId;
      row.Producto = it.Producto;
      row.Unidad = it.Unidad;
      row.CantidadNeta = it.CantidadNeta;
      row.MermaFactor = it.MermaFactor;
      row.PrecioUnit = it.PrecioUnit;
      row.CostoAdicional = it.CostoAdicional;

      // conversión al sistema internacional (solo si aplica)
      var unidadNorm = units.Normalize(it.Unidad);
      if (unidadNorm != "unid" && units.TryConvert(it.CantidadNeta, it.Unidad, unidadNorm, out var converted))
        row.CantidadNeta = converted;

      row.Subtotal = row.CantidadNeta * row.MermaFactor * row.PrecioUnit + row.CostoAdicional;
    }

    await db.SaveChangesAsync();
    return Ok();
  }

  // GET: api/workbooks/5/summary
  [HttpGet("{id}/summary")]
  public async Task<ActionResult<WorkbookSummaryDto>> Summary(int id)
    => Ok(await svc.GetSummaryAsync(id));

  // GET: api/workbooks/5/pile
  [HttpGet("{id}/pile")]
  public async Task<ActionResult<IEnumerable<WorkbookPileItemDto>>> Pile(
    int id, string? q, string order = "cost", string dir = "desc")
    => Ok(await svc.GetPileAsync(id, q, order, dir));
}
