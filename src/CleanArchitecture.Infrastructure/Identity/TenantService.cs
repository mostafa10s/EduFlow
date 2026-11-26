using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Identity;

public class TenantService : ITenantService
{
    private readonly ApplicationDbContext _context;

    public TenantService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> CreateTenantAsync(
        string name,
        string slug,
        TenantType type,
        string ownerUserId,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var tenant = new Tenant
            {
                Name = name,
                Slug = slug,
                Type = type,
                IsActive = true
            };

            await _context.Tenants.AddAsync(
                tenant,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var membership = new TenantMember
            {
                TenantId = tenant.Id,
                UserId = ownerUserId,
                Role = TenantMemberRole.Owner,
                IsActive = true
            };

            await _context.TenantMembers.AddAsync(
                membership,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return tenant.Id;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
    public async Task<IList<Tenant>> GetMyTenantsAsync(
    string userId,
    CancellationToken cancellationToken)
    {
        return await _context.TenantMembers
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive)
            .Join(
                _context.Tenants.Where(x => x.IsActive),
                member => member.TenantId,
                tenant => tenant.Id,
                (member, tenant) => tenant)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}