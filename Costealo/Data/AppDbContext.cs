using Microsoft.EntityFrameworkCore;
using Costealo.Models;

namespace Costealo.Data;

public class AppDbContext(DbContextOptions<AppDbContext> opt) : DbContext(opt)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<PriceDatabase> PriceDatabases => Set<PriceDatabase>();
    public DbSet<PriceItem> PriceItems => Set<PriceItem>();
    public DbSet<Workbook> Workbooks => Set<Workbook>();
    public DbSet<WorkbookItem> WorkbookItems => Set<WorkbookItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<RefreshToken>().HasIndex(r => r.Token);

        m.Entity<PriceItem>().Property(p => p.Precio).HasPrecision(18,4);
        m.Entity<WorkbookItem>(e =>
        {
            e.Property(p => p.PrecioUnit).HasPrecision(18,4);
            e.Property(p => p.CostoAdicional).HasPrecision(18,4);
            e.Property(p => p.Subtotal).HasPrecision(18,4);
        });
        m.Entity<Workbook>(e =>
        {
            e.Property(p => p.IvaPct).HasPrecision(6,4);
            e.Property(p => p.CostoOperativosPct).HasPrecision(6,4);
            e.Property(p => p.CostoOperativosMonto).HasPrecision(18,4);
            e.Property(p => p.PrecioVentaActualUnit).HasPrecision(18,4);
        });
    }
}