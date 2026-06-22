using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

/// <summary>
/// Product-specific queries that don't belong in the generic repository.
/// Keeps the Domain free of EF Core concerns.
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetInStockAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetWithStockCheckAsync(int id, int requestedQuantity,
                                          CancellationToken cancellationToken = default);
}
