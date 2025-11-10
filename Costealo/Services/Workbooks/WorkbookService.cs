using Costealo.Data;
using Costealo.Dtos.Workbooks;
using Costealo.Models;
using Costealo.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Costealo.Services.Workbooks;

public class WorkbookService(AppDbContext db, IUnitConversionService units) : IWorkbookService
{
  private static decimal Sub(WorkbookItem i) =>
    i.CantidadNeta * i.MermaFactor * i.PrecioUnit + i.CostoAdicional;

  public async Task<WorkbookSummaryDto> GetSummaryAsync(int id)
  {
    var w = await db.Workbooks.Include(x => x.Items).FirstAsync(x => x.Id == id);
    var u = Math.Max(1, w.Unidades);

    var subt = w.Items.Select(Sub).ToList();
    var costoIng = subt.Sum();
    var pesoTot = w.Items.Sum(i => i.CantidadNeta);
    var costoUnit = costoIng / u;

    var addTotal = w.Items.Sum(i => i.CostoAdicional);
    var opUnit = (w.CostoOperativosPct ?? 0m) * costoUnit +
                 (w.CostoOperativosMonto ?? 0m) / u;

    var ctu = costoUnit + (addTotal / u) + opUnit;
    var ct = ctu * u;
    var cfu = ctu * (1 + w.IvaPct);
    var cft = cfu * u;

    decimal? margenActual = null;
    if (w.PrecioVentaActualUnit is not null && cfu > 0)
      margenActual = (w.PrecioVentaActualUnit.Value - cfu) / cfu;

    decimal? sugerido = null;
    if (w.MargenObjetivoPct is not null)
      sugerido = cfu * (1 + w.MargenObjetivoPct.Value);

    return new WorkbookSummaryDto
    {
      WorkbookId = w.Id,
      Moneda = w.Moneda,
      Unidades = u,
      CostoIngredientes = Math.Round(costoIng, 4),
      PesoTotal = Math.Round(pesoTot, 4),
      PesoUnitario = Math.Round(pesoTot / u, 4),
      CostosAdicionalesTotal = Math.Round(addTotal, 4),
      CostosOperativosUnit = Math.Round(opUnit, 4),
      CostoUnitario = Math.Round(costoUnit, 4),
      CostoTotalUnitario = Math.Round(ctu, 4),
      CostoTotal = Math.Round(ct, 4),
      CostoFinalUnitario = Math.Round(cfu, 4),
      CostoFinal = Math.Round(cft, 4),
      IvaPct = w.IvaPct,
      MargenObjetivoPct = w.MargenObjetivoPct,
      PrecioVentaActualUnit = w.PrecioVentaActualUnit,
      PrecioVentaSugeridoUnit = sugerido,
      MargenActualPct = margenActual
    };
  }

  public async Task<IReadOnlyList<WorkbookPileItemDto>> GetPileAsync(
    int id, string? q, string order, string dir)
  {
    var w = await db.Workbooks.Include(x => x.Items).FirstAsync(x => x.Id == id);
    var u = Math.Max(1, w.Unidades);
    var precioAct = w.PrecioVentaActualUnit ?? 0m;

    var items = w.Items.AsEnumerable();
    if (!string.IsNullOrWhiteSpace(q))
      items = items.Where(i => i.Producto.Contains(q, StringComparison.OrdinalIgnoreCase));

    var subtAll = w.Items.Select(Sub).ToList();
    var sumSub = subtAll.Sum();
    var costoUnit = sumSub / u;
    var opUnit = (w.CostoOperativosPct ?? 0m) * costoUnit +
                 (w.CostoOperativosMonto ?? 0m) / u;

    var list = items.Select(i =>
    {
      var sub = Sub(i);
      var peso = sumSub > 0 ? sub / sumSub : 0m;
      var ctu_i = (sub / u) + (opUnit * peso);
      var cfu_i = ctu_i * (1 + w.IvaPct);
      var margen = cfu_i > 0 ? (precioAct - cfu_i) / cfu_i : 0m;
      return new WorkbookPileItemDto(
        i.Producto, Math.Round(cfu_i, 4), precioAct, Math.Round(margen, 4));
    });

    list = order == "name"
      ? (dir == "asc" ? list.OrderBy(x => x.Producto) : list.OrderByDescending(x => x.Producto))
      : (dir == "asc" ? list.OrderBy(x => x.CostoFinalUnit)
                      : list.OrderByDescending(x => x.CostoFinalUnit));

    return list.ToList();
  }
}
