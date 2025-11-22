using System.Security.Claims;
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
public class DatabasesController(
    AppDbContext db,
    IBlobService blob,
    IExcelParser parser,
    IImportValidationService validator,
    IUrlImportService urlImport) : ControllerBase
{
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    // GET: api/databases
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PriceDatabaseDto>>> GetAll()
    {
        var userId = GetUserId();
        var databases = await db.PriceDatabases
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

        return Ok(databases.Select(MapToDto));
    }

    // GET: api/databases/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PriceDatabaseDto>> GetById(int id)
    {
        var userId = GetUserId();
        var database = await db.PriceDatabases.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

        if (database == null) return NotFound();

        return Ok(MapToDto(database));
    }

    // POST: api/databases
    [HttpPost]
    public async Task<ActionResult<PriceDatabaseDto>> Create(CreateDatabaseDto dto)
    {
        var userId = GetUserId();
        var database = new PriceDatabase
        {
            UserId = userId,
            Nombre = dto.Nombre,
            FileType = "manual",
            IsValidated = true
        };

        db.PriceDatabases.Add(database);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = database.Id }, MapToDto(database));
    }

    // PUT: api/databases/5
    [HttpPut("{id}")]
    public async Task<ActionResult<PriceDatabaseDto>> Update(int id, UpdateDatabaseDto dto)
    {
        var userId = GetUserId();
        var database = await db.PriceDatabases.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

        if (database == null) return NotFound();

        database.Nombre = dto.Nombre;
        await db.SaveChangesAsync();

        return Ok(MapToDto(database));
    }

    // DELETE: api/databases/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var database = await db.PriceDatabases
            .Include(d => d.Items)
            .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

        if (database == null) return NotFound();

        db.PriceItems.RemoveRange(database.Items);
        db.PriceDatabases.Remove(database);
        await db.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/databases/upload
    [HttpPost("upload")]
    public async Task<ActionResult> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0) return BadRequest("Archivo requerido.");

        var userId = GetUserId();
        var errors = new List<ValidationErrorDto>();
        var validItems = new List<(string id, string producto, string unidad, decimal precio, string moneda)>();

        // Parse file
        var rows = parser.Read(file.OpenReadStream(), Path.GetExtension(file.FileName));
        int rowNumber = 1; // Start from row 1 (assuming header is row 0)

        foreach (var (id, producto, unidad, precio, moneda) in rows)
        {
            rowNumber++;
            bool rowValid = true;

            // Validate required fields
            rowValid &= validator.ValidateRequiredField(id, "ID", rowNumber, errors);
            rowValid &= validator.ValidateRequiredField(producto, "Producto", rowNumber, errors);
            rowValid &= validator.ValidateRequiredField(unidad, "Unidad", rowNumber, errors);

            // Validate price
            var (priceValid, priceValue) = validator.ValidatePrice(precio.ToString(), rowNumber, errors);
            rowValid &= priceValid;

            // Validate currency
            var currencyValue = validator.ValidateCurrency(moneda, rowNumber, errors);
            if (!errors.Any(e => e.RowNumber == rowNumber && e.FieldName == "Moneda"))
            {
                // Currency is valid
            }
            else
            {
                rowValid = false;
            }

            if (rowValid)
            {
                validItems.Add((id.Trim(), producto.Trim(), unidad.Trim(), priceValue, currencyValue));
            }
        }

        // If there are errors, return them to the user
        if (errors.Any())
        {
            var result = new ImportValidationResultDto(
                IsValid: false,
                Errors: errors,
                TotalRows: rowNumber - 1,
                ValidRows: validItems.Count,
                InvalidRows: errors.Select(e => e.RowNumber).Distinct().Count()
            );

            return BadRequest(result);
        }

        // All valid - save to database
        var blobUrl = await blob.UploadAsync(file.OpenReadStream(), file.ContentType, file.FileName);
        var pd = new PriceDatabase
        {
            UserId = userId,
            Nombre = Path.GetFileNameWithoutExtension(file.FileName),
            FileName = file.FileName,
            FileType = Path.GetExtension(file.FileName).TrimStart('.'),
            BlobUrl = blobUrl,
            RowCount = validItems.Count,
            IsValidated = true
        };

        db.PriceDatabases.Add(pd);
        await db.SaveChangesAsync();

        foreach (var (id, producto, unidad, precio, moneda) in validItems)
        {
            db.PriceItems.Add(new PriceItem
            {
                PriceDatabaseId = pd.Id,
                ExtId = id,
                Producto = producto.ToUpperInvariant(),
                Unidad = unidad.ToLowerInvariant(),
                Precio = precio,
                Moneda = moneda
            });
        }

        await db.SaveChangesAsync();

        return Ok(MapToDto(pd));
    }

    // POST: api/databases/import-url
    [HttpPost("import-url")]
    public async Task<ActionResult> ImportFromUrl(ImportFromUrlDto dto)
    {
        var userId = GetUserId();
        var errors = new List<ValidationErrorDto>();
        var validItems = new List<(string id, string producto, string unidad, decimal precio, string moneda)>();

        Stream fileStream;
        try
        {
            fileStream = await urlImport.DownloadFileAsync(dto.Url);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "No se pudo descargar el archivo desde la URL", details = ex.Message });
        }

        // Parse file
        var fileType = Path.GetExtension(dto.Url);
        var rows = parser.Read(fileStream, fileType);
        int rowNumber = 1;

        foreach (var (id, producto, unidad, precio, moneda) in rows)
        {
            rowNumber++;
            bool rowValid = true;

            rowValid &= validator.ValidateRequiredField(id, "ID", rowNumber, errors);
            rowValid &= validator.ValidateRequiredField(producto, "Producto", rowNumber, errors);
            rowValid &= validator.ValidateRequiredField(unidad, "Unidad", rowNumber, errors);

            var (priceValid, priceValue) = validator.ValidatePrice(precio.ToString(), rowNumber, errors);
            rowValid &= priceValid;

            var currencyValue = validator.ValidateCurrency(moneda, rowNumber, errors);
            if (errors.Any(e => e.RowNumber == rowNumber && e.FieldName == "Moneda"))
            {
                rowValid = false;
            }

            if (rowValid)
            {
                validItems.Add((id.Trim(), producto.Trim(), unidad.Trim(), priceValue, currencyValue));
            }
        }

        if (errors.Any())
        {
            var result = new ImportValidationResultDto(
                IsValid: false,
                Errors: errors,
                TotalRows: rowNumber - 1,
                ValidRows: validItems.Count,
                InvalidRows: errors.Select(e => e.RowNumber).Distinct().Count()
            );

            return BadRequest(result);
        }

        // Save database with source URL
        var blobUrl = await blob.UploadAsync(fileStream, "application/octet-stream", Path.GetFileName(dto.Url));
        var pd = new PriceDatabase
        {
            UserId = userId,
            Nombre = dto.Nombre,
            FileName = Path.GetFileName(dto.Url),
            FileType = fileType.TrimStart('.'),
            BlobUrl = blobUrl,
            SourceUrl = dto.Url,
            LastRefreshedAt = DateTime.UtcNow,
            RowCount = validItems.Count,
            IsValidated = true
        };

        db.PriceDatabases.Add(pd);
        await db.SaveChangesAsync();

        foreach (var (id, producto, unidad, precio, moneda) in validItems)
        {
            db.PriceItems.Add(new PriceItem
            {
                PriceDatabaseId = pd.Id,
                ExtId = id,
                Producto = producto.ToUpperInvariant(),
                Unidad = unidad.ToLowerInvariant(),
                Precio = precio,
                Moneda = moneda
            });
        }

        await db.SaveChangesAsync();

        return Ok(MapToDto(pd));
    }

    // PUT: api/databases/5/refresh
    [HttpPut("{id}/refresh")]
    public async Task<ActionResult> Refresh(int id)
    {
        var userId = GetUserId();
        var database = await db.PriceDatabases
            .Include(d => d.Items)
            .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

        if (database == null) return NotFound();
        if (string.IsNullOrEmpty(database.SourceUrl))
        {
            return BadRequest("Esta base de datos no tiene una URL de origen configurada.");
        }

        var errors = new List<ValidationErrorDto>();
        var validItems = new List<(string id, string producto, string unidad, decimal precio, string moneda)>();

        Stream fileStream;
        try
        {
            fileStream = await urlImport.DownloadFileAsync(database.SourceUrl);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "No se pudo descargar el archivo desde la URL", details = ex.Message });
        }

        // Parse file
        var fileType = Path.GetExtension(database.SourceUrl);
        var rows = parser.Read(fileStream, fileType);
        int rowNumber = 1;

        foreach (var (itemId, producto, unidad, precio, moneda) in rows)
        {
            rowNumber++;
            bool rowValid = true;

            rowValid &= validator.ValidateRequiredField(itemId, "ID", rowNumber, errors);
            rowValid &= validator.ValidateRequiredField(producto, "Producto", rowNumber, errors);
            rowValid &= validator.ValidateRequiredField(unidad, "Unidad", rowNumber, errors);

            var (priceValid, priceValue) = validator.ValidatePrice(precio.ToString(), rowNumber, errors);
            rowValid &= priceValid;

            var currencyValue = validator.ValidateCurrency(moneda, rowNumber, errors);
            if (errors.Any(e => e.RowNumber == rowNumber && e.FieldName == "Moneda"))
            {
                rowValid = false;
            }

            if (rowValid)
            {
                validItems.Add((itemId.Trim(), producto.Trim(), unidad.Trim(), priceValue, currencyValue));
            }
        }

        if (errors.Any())
        {
            var result = new ImportValidationResultDto(
                IsValid: false,
                Errors: errors,
                TotalRows: rowNumber - 1,
                ValidRows: validItems.Count,
                InvalidRows: errors.Select(e => e.RowNumber).Distinct().Count()
            );

            return BadRequest(result);
        }

        // Remove old items and add new ones
        db.PriceItems.RemoveRange(database.Items);

        foreach (var (itemId, producto, unidad, precio, moneda) in validItems)
        {
            db.PriceItems.Add(new PriceItem
            {
                PriceDatabaseId = database.Id,
                ExtId = itemId,
                Producto = producto.ToUpperInvariant(),
                Unidad = unidad.ToLowerInvariant(),
                Precio = precio,
                Moneda = moneda
            });
        }

        database.RowCount = validItems.Count;
        database.LastRefreshedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(MapToDto(database));
    }

    // GET: api/databases/5/items
    [HttpGet("{id}/items")]
    public async Task<ActionResult<IEnumerable<PriceItemDto>>> Items(int id, string? search = null, int page = 1, int pageSize = 20)
    {
        var userId = GetUserId();
        var database = await db.PriceDatabases.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
        if (database == null) return NotFound();

        var q = db.PriceItems.Where(x => x.PriceDatabaseId == id);
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x => x.Producto.Contains(search.ToUpper()) || x.ExtId.Contains(search));
        var list = await q.OrderBy(x => x.Producto).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new PriceItemDto(x.Id, x.PriceDatabaseId, x.ExtId, x.Producto, x.Unidad, x.Precio, x.Moneda)).ToListAsync();
        return Ok(list);
    }

    private static PriceDatabaseDto MapToDto(PriceDatabase db) => new(
        db.Id,
        db.Nombre,
        db.FileName,
        db.FileType,
        db.RowCount,
        db.BlobUrl,
        db.SourceUrl,
        db.LastRefreshedAt,
        db.CreatedAt
    );
}
