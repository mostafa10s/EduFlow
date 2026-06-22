using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Product>> GetInStockAsync(CancellationToken ct = default)
        => await DbSet.AsNoTracking().Where(p => p.StockQuantity > 0).ToListAsync(ct);

    public async Task<Product?> GetWithStockCheckAsync(int id, int requestedQuantity,
                                                       CancellationToken ct = default)
    {
        var product = await GetByIdAsync(id, ct);
        return product is { } p && p.IsInStock(requestedQuantity) ? p : null;
    }
}
