using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Infrastructure.Persistence;

namespace CleanArchitecture.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ProductRepository? _productRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public UnitOfWork(AppDbContext context, ProductRepository productRepository)
    {
        _context = context;
        _productRepository = productRepository;
    }

    public IProductRepository Products => _productRepository ?? new ProductRepository(_context);
    public IRepository<Customer> Customers => new Repository<Customer>(_context);
    public IRepository<Order> Orders => new Repository<Order>(_context);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);

    public void Dispose() => _context.Dispose();
}
